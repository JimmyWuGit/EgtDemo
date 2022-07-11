using EgtDemo.IRepository;
using EgtDemo.IService;
using EgtDemo.Model;
using System;

namespace EgtDemo.Service
{
    public class DemoService : IDemoService
    {
        private readonly IDemoRepository _demoRepository;

        public DemoService(IDemoRepository demoRepository)
        {
            _demoRepository = demoRepository;
        }

        public Demo GetDemo()
        {
            return _demoRepository.GetDemo();
        }
    }
}
