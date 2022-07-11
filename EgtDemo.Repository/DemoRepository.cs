using EgtDemo.IRepository;
using EgtDemo.Model;
using System;

namespace EgtDemo.Repository
{
    public class DemoRepository : IDemoRepository
    {
        public Demo GetDemo()
        {
            return new Demo()
            {
                Id = 1,
                Name = "JimmyWu"
            };
        }
    }
}
