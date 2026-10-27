using Android.App;
using Android.OS;
using Kapusch.PostHog.Android;

[Activity(Label = "PostHog smoke", MainLauncher = true)]
public class MainActivity : Activity
{
    protected override void OnCreate(Bundle? state)
    {
        base.OnCreate(state);
        // Start opted out, as an app waiting for consent would. The persisted choice is disabled so
        // every launch exercises the same path.
        NativePostHog.Configure(
            this,
            "phc_sample",
            "https://127.0.0.1:1",
            new NativePostHogOptions { Debug = true, OptOut = true, PersistOptOut = false }
        );
        var startedOptedOut = NativePostHog.IsOptOut;
        NativePostHog.Capture("native_smoke_dropped", "{\"source\":\"sample\"}");
        NativePostHog.OptIn();
        NativePostHog.Capture("native_smoke", "{\"source\":\"sample\"}");
        Android.Util.Log.Info(
            "PostHogSmoke",
            "POSTHOG_NATIVE_SMOKE_OK "
                + NativePostHog.DistinctId
                + " startedOptedOut="
                + startedOptedOut
                + " optedOutAfterOptIn="
                + NativePostHog.IsOptOut
        );
        NativePostHog.Flush();
    }
}
