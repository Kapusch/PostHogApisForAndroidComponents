namespace Kapusch.PostHog.Android;

/// <summary>Setup options for <see cref="NativePostHog.Configure(global::Android.Content.Context, string, string, NativePostHogOptions)"/>.</summary>
public sealed class NativePostHogOptions
{
    /// <summary>Enables SDK debug logs. Logs may contain event properties; keep off in production.</summary>
    public bool Debug { get; init; }

    /// <summary>Captures Application Installed/Updated/Opened/Backgrounded events. Default: true.</summary>
    public bool CaptureLifecycle { get; init; } = true;

    /// <summary>
    /// Starts the SDK opted out: nothing is captured or sent until <see cref="NativePostHog.OptIn"/>.
    /// Default: false (upstream default, capture enabled). When <see cref="PersistOptOut"/> is true,
    /// a runtime <see cref="NativePostHog.OptIn"/>/<see cref="NativePostHog.OptOut"/> choice stored by
    /// an earlier launch takes precedence over this value.
    /// </summary>
    public bool OptOut { get; init; }

    /// <summary>
    /// Whether the SDK stores the runtime opt-in/opt-out choice and restores it at the next setup.
    /// Default: true (upstream default). Set false only when the app owns its consent store and
    /// passes <see cref="OptOut"/> from it at every launch; the SDK then neither reads nor writes
    /// its own copy. Backed by an upstream property marked internal; see Docs/Consent.md.
    /// </summary>
    public bool PersistOptOut { get; init; } = true;
}
