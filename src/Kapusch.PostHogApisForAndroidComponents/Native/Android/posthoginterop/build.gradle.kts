plugins { id("com.android.library"); kotlin("android") }
android {
 namespace = "com.kapusch.posthog.androidinterop"
 compileSdk = 35
 defaultConfig { minSdk = 21; consumerProguardFiles("consumer-rules.pro") }
 compileOptions { sourceCompatibility = JavaVersion.VERSION_17; targetCompatibility = JavaVersion.VERSION_17 }
 kotlinOptions { jvmTarget = "17" }
}
dependencies { implementation("com.posthog:posthog-android:3.38.2") }
// Other dependencies are supplied by managed NuGets to avoid duplicate Java classes.
tasks.register<Copy>("copyNativeDependencies") {
 from(configurations.getByName("releaseRuntimeClasspath")) {
  include("posthog-6.8.1.jar", "posthog-android-3.38.2.aar", "curtains-1.2.5.aar")
 }
 into(rootProject.layout.projectDirectory.dir("build/deps"))
}
