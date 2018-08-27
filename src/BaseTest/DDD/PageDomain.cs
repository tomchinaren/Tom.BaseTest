using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btest.DDD
{
    public class PageDomainDemo
    {
        public static void Run()
        {
            var page = new PageDomain();
            page.SetContainer(new PageDomain.PageContainer { Name = "999" });
            page.AddItem(new PageDomain.Fp() { Name = "1" });
            page.AddItem(new PageDomain.Menu() { Name = "1" });
            page.AddItem(new PageDomain.Menu() { Name = "2" });
            page.AddItem(new PageDomain.Fp() { Name = "2" });
            page.Save();
        }
    }

    /// <summary>
    /// 要+命名叫BO
    /// </summary>
    public partial class PageDomain
    {
        private PageContainer Container;
        private List<PageItem> Items = new List<PageItem>();

        public void SetContainer(PageContainer container)
        {
            Container = container;
        }
        public PageContainer GetContainer()
        {
            return Container;
        }
        public void AddItem(PageItem item)
        {
            Items.Add(item);
        }
        public List<PageItem> GetItems()
        {
            return Items;
        }

        public void Save()
        {
            var repo = new PageDomainRepo();
            repo.Save(this);
        }
    }

    public partial class PageDomain
    {
        public class PageContainer
        {
            public string Name;
        }

        public class PageItem
        {
            public int Type;
            public string Name;
        }

        public class Fp : PageItem
        {
            public Fp()
            {
                Type = 1;
            } 
        }
        public class Menu : PageItem
        {
            public Menu()
            {
                Type = 2;
            }

        }

        private class PageDomainRepo
        {
            PageContainerRepo PageContainerRepo;
            FpRepo FpRepo;
            MenuRepo MenuRepo;
            Dictionary<int, IRepo> DictRepo;

            public PageDomainRepo()
            {
                PageContainerRepo = new PageContainerRepo();
                FpRepo = new FpRepo();
                MenuRepo = new MenuRepo();

                DictRepo = new Dictionary<int, IRepo>() {
                    { 1,FpRepo },
                    { 2,MenuRepo },
                };
            }

            public void Save(PageDomain page)
            {
                Console.WriteLine("PageDomainRepo Begin Saving");

                var container = page.GetContainer();
                PageContainerRepo.Save(container);

                var items = page.GetItems();
                var vItems = GetVItems(items);
                foreach(var vitem in vItems)
                {
                    vitem.Save();
                }
            }

            private List<VPageItem> GetVItems(List<PageItem> items)
            {
                var vItems = items.Select(t => new VPageItem(t, DictRepo[t.Type])).ToList();
                return vItems;
            }
        }

        private class PageContainerRepo
        {
            public void Save(PageContainer container)
            {
                Console.WriteLine("PageContainerRepo Save {0}", container.Name);
            }
        }

        private class VPageItem
        {
            private PageItem Item;
            private IRepo Repo;
            public VPageItem(PageItem item, IRepo repo)
            {
                Item = item;
                Repo = repo;
            }
            public void Save()
            {
                Repo.Save(Item);
            }
        }

        interface IRepo
        {
            void Save(PageItem item);
        }

        private class FpRepo: IRepo
        {
            public void Save(PageItem item)
            {
                Console.WriteLine("FpRepo Save {0}", item.Name);
            }
        }
        private class MenuRepo : IRepo
        {
            public void Save(PageItem item)
            {
                Console.WriteLine("MenuRepo Save {0}", item.Name);
            }
        }

    }


}
