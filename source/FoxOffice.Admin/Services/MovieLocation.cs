namespace FoxOffice.Admin.Services
{
    using System;

    public readonly struct MovieLocation
    {
        public MovieLocation(Uri uri) => Uri = uri;

        public Uri Uri { get; }
    }
}
