# Napoli's OpenTelemetry Extensions

This Nuget package contains some useful helpers and components to setup a tracing instrumentation with features like:
- Dynamic updatable configuration.
- Debug mode header (which forces sampling).
- Recorded request traces throttling (limit the amount of ongoing traces).
- Per route and entrypoint operations probabilistic sampling.
- Sampling metrics.
- Easy AWS Cloud and Host information enrichment to OTLP's Resources.