using System.Collections.Generic;
using ProjectHeleus.SharedLibrary.Models;

namespace ProjectHeleus.SharedLibrary.Providers.Menu.Contracts
{
    public interface IMenuProvider
    {
        IEnumerable<MenuItem> GetMainItems();
        IEnumerable<MenuItem> GetOptionItems();
    }
}