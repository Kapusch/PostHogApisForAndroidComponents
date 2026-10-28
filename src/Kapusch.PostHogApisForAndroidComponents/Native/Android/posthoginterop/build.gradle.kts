plugins { id("com.android.library"); kotlin("android") }
android {
 namespace = "com.kapusch.posthog.androidinterop"
 compileSdk = 35
 // PostHog Android raised its minSdk to 23; the wrapper cannot be lower than its dependency.
 defaultConfig { minSdk = 23; consumerProguardFiles("consumer-rules.pro") }
 compileOptions { sourceCompatibility = JavaVersion.VERSION_17; targetCompatibility = JavaVersion.VERSION_17 }
 kotlinOptions { jvmTarget = "17" }
}
dependencies { implementation("com.posthog:posthog-android:3.73.1") }
// Other dependencies are supplied by managed NuGets to avoid duplicate Java classes.
tasks.register<Copy>("copyNativeDependencies") {
 from(configurations.getByName("releaseRuntimeClasspath")) {
  include("posthog-6.47.7.jar", "posthog-android-3.73.1.aar", "curtains-1.2.5.aar")
 }
 into(rootProject.layout.projectDirectory.dir("build/deps"))
}
