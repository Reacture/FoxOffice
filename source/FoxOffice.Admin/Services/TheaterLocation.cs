namespace FoxOffice.Admin.Services
{
    using System;

    public readonly struct TheaterLocation
    {
        public TheaterLocation(Uri uri) => Uri = uri;

        public Uri Uri { get; }
    }
}
