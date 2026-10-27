package com.kapusch.posthog.androidinterop

import android.content.Context
import com.posthog.PostHog
import com.posthog.android.PostHogAndroid
import com.posthog.android.PostHogAndroidConfig
import org.json.JSONObject
import org.json.JSONArray

/** Thin JNI surface; the upstream SDK owns the disk queue and lifecycle. */
object PostHogInterop {
 /** Orders reset() against optIn()/optOut(); always taken before any upstream lock. */
 private val consentLock = Any()
 /**
  * [optOut] is the consent state used until a persisted runtime choice is read (or always, when
  * [persistOptOut] is false). `persistOptOut` is annotated `@PostHogInternal` upstream (since
  * 3.66.0): it is honored by the pinned SDK but carries no upstream stability guarantee.
  */
 @JvmStatic fun setup(context: Context, key: String, host: String, debug: Boolean, lifecycle: Boolean, optOut: Boolean, persistOptOut: Boolean) {
  require(key.startsWith("phc_") && android.net.Uri.parse(host).scheme == "https")
  val config = PostHogAndroidConfig(projectToken = key, host = host)
  config.debug = debug
  config.captureApplicationLifecycleEvents = lifecycle
  config.optOut = optOut
  config.persistOptOut = persistOptOut
  config.captureScreenViews = false
  config.captureDeepLinks = false
  config.preloadFeatureFlags = false
  // Remote config only drives replay, surveys, error tracking and push, none of which this wrapper
  // exposes. Disabling it removes the setup-time request, which is sent even while opted out.
  // Deprecated upstream as "always enabled", but 3.73.1 still reads it in setup(); recheck on bumps.
  @Suppress("DEPRECATION")
  config.remoteConfig = false
  config.sessionReplay = false
  // Push token registration and open capture are on upstream and auto-arm when Firebase Messaging
  // is on the classpath. This wrapper neither exposes nor qualifies them.
  config.capturePushNotificationSubscriptions = false
  config.capturePushNotificationOpened = false
  config.flushAt = 20
  config.maxQueueSize = 1000
  config.flushIntervalSeconds = 5
  PostHogAndroid.setup(context.applicationContext, config)
 }
 @JvmStatic fun capture(event: String, json: String) {
  require(event.isNotBlank())
  PostHog.capture(event, properties = map(JSONObject(json)))
 }
 @JvmStatic fun identify(id: String) { require(id.isNotBlank()); PostHog.identify(id) }
 /**
  * Upstream reset() clears every stored value except a fixed list that does not include the
  * persisted opt-out, so a later launch would fall back to the setup default. Re-apply an active
  * opt-out after the reset so consent withdrawal survives logout (fail closed). An opted-in state is
  * not re-persisted: it falls back to the setup default instead. Runs under [consentLock] so a
  * concurrent optIn()/optOut() cannot land between the read and the re-applied opt-out.
  */
 @JvmStatic fun reset() {
  synchronized(consentLock) {
   val optedOut = PostHog.isOptOut()
   PostHog.reset()
   if (optedOut) PostHog.optOut()
  }
 }
 @JvmStatic fun flush() { PostHog.flush() }
 @JvmStatic fun distinctId(): String = PostHog.distinctId()
 @JvmStatic fun optIn() { synchronized(consentLock) { PostHog.optIn() } }
 @JvmStatic fun optOut() { synchronized(consentLock) { PostHog.optOut() } }
 @JvmStatic fun isOptOut(): Boolean = PostHog.isOptOut()
 private fun map(json: JSONObject): Map<String, Any> = json.keys().asSequence().associateWith { value(json.get(it)) }
 private fun value(v: Any): Any = when (v) {
  is JSONObject -> map(v)
  is JSONArray -> (0 until v.length()).map { value(v.get(it)) }
  JSONObject.NULL -> throw IllegalArgumentException("JSON null properties are not supported; omit missing properties")
  else -> v
 }
}
