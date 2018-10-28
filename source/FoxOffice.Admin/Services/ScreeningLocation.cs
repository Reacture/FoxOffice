namespace FoxOffice.Admin.Services
{
    using System;

    public readonly struct ScreeningLocation
    {
        public ScreeningLocation(Uri uri) => Uri = uri;

        public Uri Uri { get; }
    }
}
