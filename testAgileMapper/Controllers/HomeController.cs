using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AgileObjects.AgileMapper;
using AgileObjects.AgileMapper.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace testAgileMapper.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Route("/")]
        public IActionResult Index()
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = "name"
            };

            var customerDto = Mapper.Map(customer).ToANew<CustomerDto>();
            
            Console.WriteLine("--- to a new ---");
            Console.WriteLine(JsonSerializer.Serialize(customerDto));

            var customerDto2 = Mapper.Map(customer).ToANew<CustomerDto>(configurator => configurator
                                                                                        .Map((c, dto) => c.Id)
                                                                                        .To(d => d.Abc));

            Console.WriteLine("--- to a new config ---");
            Console.WriteLine(JsonSerializer.Serialize(customerDto2));

            var target = new Customer("abc");
            Mapper.Map(customer).Over(target);

            Console.WriteLine("--- over ---");
            Console.WriteLine(JsonSerializer.Serialize(target));

            var one = new List<Customer>
            {
                new Customer("abc")
            };

            var two = new List<Customer>
            {
                new Customer("123")
            };

            Mapper.Map(one).OnTo(two);

            Console.WriteLine("--- on to ---");
            Console.WriteLine("one --" + JsonSerializer.Serialize(one));
            Console.WriteLine("two --" + JsonSerializer.Serialize(two));

            var cc = new Customer("abc");
            var ccClone = cc.DeepClone();

            var referenceEquals = ReferenceEquals(cc, ccClone);

            Console.WriteLine("--- deep clone ---");
            Console.WriteLine($"is equal - {referenceEquals}");
            Console.WriteLine("origin --" + JsonSerializer.Serialize(cc));
            Console.WriteLine("clone --" + JsonSerializer.Serialize(ccClone));
            
            var plan = Mapper.GetPlanFor<Customer>().ToANew<CustomerDto>();

            Console.WriteLine("--- plan ---");
            Console.WriteLine(plan);

            return Ok();
        }

        [Route("/db")]
        public async Task<IActionResult> Db()
        {
            // var result = await _dbContext.User
            //                              .Project()
            //                              .To<CustomerDto>()
            //                              .ToListAsync();
            //
            // Console.WriteLine("--- db project ---");
            // Console.WriteLine(JsonSerializer.Serialize(result));
            
            var mapper = Mapper.CreateNew();
            
            mapper.WhenMapping
                  .From<User>()
                  .ProjectedTo<CustomerDto>()
                  .Map(a => a.Name)
                  .To(a => a.Abc);
            
            var result2 = await _dbContext.User
                                          .ProjectUsing(mapper)
                                          .To<CustomerDto>()
                                          .ToListAsync();
            
            Console.WriteLine("--- db project config instance ---");
            Console.WriteLine(JsonSerializer.Serialize(result2));

            Mapper.WhenMapping
                  .From<User>()
                  .To<CustomerDto>()
                  .Map((u, dto) => u.Name)
                  .To(a => a.Abc);

            var result3 = await _dbContext.User
                                          .Project()
                                          .To<CustomerDto>()
                                          .ToListAsync();

            Console.WriteLine("--- db project config static ---");
            Console.WriteLine(JsonSerializer.Serialize(result3));

            var plan = Mapper.GetPlanForProjecting(_dbContext.User).To<CustomerDto>();

            Console.WriteLine("--- plan ---");
            Console.WriteLine(plan);
            
            return Ok();
        }
    }

    public class CustomerDto
    {
        public string Id { get; set; }

        // public string Name { get; set; }

        public string Abc { get; set; }
    }

    public class Customer
    {
        public Customer(string name)
        {
            Name = name;
            Id = Guid.NewGuid();
        }

        public Customer()
        {
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}