namespace ProjectHeleus.WindowsLibrary.Providers.Menu.Contracts
{
    using System.Collections.Generic;

    using Models;

    public interface IMenuProvider
    {
        IEnumerable<MenuItem> GetMainItems();
        IEnumerable<MenuItem> GetOptionItems();
    }
}