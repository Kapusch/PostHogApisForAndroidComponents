using Android.Content;
using Android.Runtime;

namespace Kapusch.PostHog.Android;

/// <summary>Thin JNI calls. Upstream owns persistence; flush is not an ingestion receipt.</summary>
public static class NativePostHog
{
    private static readonly IntPtr Class = JNIEnv.FindClass(
        "com/kapusch/posthog/androidinterop/PostHogInterop"
    );

    public static void Configure(
        Context context,
        string projectKey,
        string host,
        bool debug = false,
        bool captureLifecycle = true
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(host);
        var key = JNIEnv.NewString(projectKey);
        var endpoint = JNIEnv.NewString(host);
        try
        {
            JNIEnv.CallStaticVoidMethod(
                Class,
                JNIEnv.GetStaticMethodID(
                    Class,
                    "setup",
                    "(Landroid/content/Context;Ljava/lang/String;Ljava/lang/String;ZZ)V"
                ),
                new JValue(context),
                new JValue(key),
                new JValue(endpoint),
                new JValue(debug),
                new JValue(captureLifecycle)
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

    public static void Reset() => Call("reset");

    public static void Flush() => Call("flush");

    public static void OptIn() => Call("optIn");

    public static void OptOut() => Call("optOut");
}
