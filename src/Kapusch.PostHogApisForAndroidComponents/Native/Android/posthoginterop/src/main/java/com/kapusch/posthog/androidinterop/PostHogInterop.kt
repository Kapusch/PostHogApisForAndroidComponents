package com.kapusch.posthog.androidinterop

import android.content.Context
import com.posthog.PostHog
import com.posthog.android.PostHogAndroid
import com.posthog.android.PostHogAndroidConfig
import org.json.JSONObject
import org.json.JSONArray

/** Thin JNI surface; the upstream SDK owns the disk queue and lifecycle. */
object PostHogInterop {
 @JvmStatic fun setup(context: Context, key: String, host: String, debug: Boolean, lifecycle: Boolean) {
  require(key.startsWith("phc_") && android.net.Uri.parse(host).scheme == "https")
  val config = PostHogAndroidConfig(key, host)
  config.debug = debug
  config.captureApplicationLifecycleEvents = lifecycle
  config.captureScreenViews = false
  config.captureDeepLinks = false
  config.preloadFeatureFlags = false
  config.sessionReplay = false
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
 @JvmStatic fun reset() { PostHog.reset() }
 @JvmStatic fun flush() { PostHog.flush() }
 @JvmStatic fun distinctId(): String = PostHog.distinctId()
 @JvmStatic fun optIn() { PostHog.optIn() }
 @JvmStatic fun optOut() { PostHog.optOut() }
 private fun map(json: JSONObject): Map<String, Any> = json.keys().asSequence().associateWith { value(json.get(it)) }
 private fun value(v: Any): Any = when (v) {
  is JSONObject -> map(v)
  is JSONArray -> (0 until v.length()).map { value(v.get(it)) }
  JSONObject.NULL -> throw IllegalArgumentException("JSON null properties are not supported; omit missing properties")
  else -> v
 }
}
