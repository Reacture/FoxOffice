namespace FoxOffice.ReadModel
{
    using System;

    public abstract class Entity
    {
        public Guid Id { get; set; }

        public string ETag { get; set; }
    }
}
