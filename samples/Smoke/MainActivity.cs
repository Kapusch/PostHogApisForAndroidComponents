using Android.App;
using Android.OS;
using Kapusch.PostHog.Android;

[Activity(Label = "PostHog smoke", MainLauncher = true)]
public class MainActivity : Activity
{
    protected override void OnCreate(Bundle? state)
    {
        base.OnCreate(state);
        NativePostHog.Configure(this, "phc_sample", "https://127.0.0.1:1", debug: true);
        NativePostHog.Capture("native_smoke", "{\"source\":\"sample\"}");
        Android.Util.Log.Info(
            "PostHogSmoke",
            "POSTHOG_NATIVE_SMOKE_OK " + NativePostHog.DistinctId
        );
        NativePostHog.Flush();
    }
}
