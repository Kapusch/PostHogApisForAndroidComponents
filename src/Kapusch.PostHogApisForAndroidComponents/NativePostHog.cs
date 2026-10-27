using Android.Content;
using Android.Runtime;

namespace Kapusch.PostHog.Android;

/// <summary>Thin JNI calls. Upstream owns persistence; flush is not an ingestion receipt.</summary>
public static class NativePostHog
{
    private static readonly IntPtr Class = JNIEnv.FindClass(
        "com/kapusch/posthog/androidinterop/PostHogInterop"
    );

    /// <summary>Configures the SDK with default options (capture enabled, upstream default).</summary>
    public static void Configure(
        Context context,
        string projectKey,
        string host,
        bool debug = false,
        bool captureLifecycle = true
    ) =>
        Configure(
            context,
            projectKey,
            host,
            new NativePostHogOptions { Debug = debug, CaptureLifecycle = captureLifecycle }
        );

    /// <summary>
    /// Configures the process-wide SDK once. Use <see cref="NativePostHogOptions.OptOut"/> to start
    /// without capturing until consent is given.
    /// </summary>
    public static void Configure(
        Context context,
        string projectKey,
        string host,
        NativePostHogOptions options
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(host);
        ArgumentNullException.ThrowIfNull(options);
        var key = JNIEnv.NewString(projectKey);
        var endpoint = JNIEnv.NewString(host);
        try
        {
            JNIEnv.CallStaticVoidMethod(
                Class,
                JNIEnv.GetStaticMethodID(
                    Class,
                    "setup",
                    "(Landroid/content/Context;Ljava/lang/String;Ljava/lang/String;ZZZZ)V"
                ),
                new JValue(context),
                new JValue(key),
                new JValue(endpoint),
                new JValue(options.Debug),
                new JValue(options.CaptureLifecycle),
                new JValue(options.OptOut),
                new JValue(options.PersistOptOut)
            );
        }
        finally
        {
            JNIEnv.DeleteLocalRef(key);
            JNIEnv.DeleteLocalRef(endpoint);
        }
    }

    public static void Capture(string name, string propertiesJson = "{}")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var ev = JNIEnv.NewString(name);
        var json = JNIEnv.NewString(propertiesJson);
        try
        {
            JNIEnv.CallStaticVoidMethod(
                Class,
                JNIEnv.GetStaticMethodID(
                    Class,
                    "capture",
                    "(Ljava/lang/String;Ljava/lang/String;)V"
                ),
                new JValue(ev),
                new JValue(json)
            );
        }
        finally
        {
            JNIEnv.DeleteLocalRef(ev);
            JNIEnv.DeleteLocalRef(json);
        }
    }

    public static void Identify(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        var value = JNIEnv.NewString(id);
        try
        {
            JNIEnv.CallStaticVoidMethod(
                Class,
                JNIEnv.GetStaticMethodID(Class, "identify", "(Ljava/lang/String;)V"),
                new JValue(value)
            );
        }
        finally
        {
            JNIEnv.DeleteLocalRef(value);
        }
    }

    public static string DistinctId =>
        JNIEnv.GetString(
            JNIEnv.CallStaticObjectMethod(
                Class,
                JNIEnv.GetStaticMethodID(Class, "distinctId", "()Ljava/lang/String;")
            ),
            JniHandleOwnership.TransferLocalRef
        ) ?? string.Empty;

    private static void Call(string name) =>
        JNIEnv.CallStaticVoidMethod(Class, JNIEnv.GetStaticMethodID(Class, name, "()V"));

    /// <summary>
    /// Clears the identity (new anonymous ID). An active opt-out is kept, including across launches.
    /// </summary>
    public static void Reset() => Call("reset");

    public static void Flush() => Call("flush");

    /// <summary>Enables capture (consent given). Persisted unless PersistOptOut is false.</summary>
    public static void OptIn() => Call("optIn");

    /// <summary>
    /// Stops capture (consent withdrawn). Persisted unless PersistOptOut is false. Events already
    /// queued before this call are not purged and may still be sent.
    /// </summary>
    public static void OptOut() => Call("optOut");

    /// <summary>True when capture is disabled, including before <see cref="Configure(Context, string, string, NativePostHogOptions)"/>.</summary>
    public static bool IsOptOut =>
        JNIEnv.CallStaticBooleanMethod(Class, JNIEnv.GetStaticMethodID(Class, "isOptOut", "()Z"));
}
