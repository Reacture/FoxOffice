namespace FoxOffice.Admin.Services
{
    using System;
    using System.Threading.Tasks;

    public interface IResourceAwaiter
    {
        Task AwaitResource(Uri location);
    }
}
