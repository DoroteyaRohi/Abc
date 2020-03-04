using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abc.Data.Quantity;
using Abc.Domain.Quantity;
using Abc.Infra.Quantity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infra
{
    public class MeasuresRepository: IMeasuresRepository
    {
        private readonly QuantityDbContext db;
        public string SortOrder { get; set; }
        public string SearchString { get; set; }

        public MeasuresRepository(QuantityDbContext c)
        {
            db = c;
        }

        public async Task<List<Measure>> Get()
        {
            var l =  await createFiltered(createSorted()).ToListAsync();
            return l.Select(e => new Measure(e)).ToList();
        }

        private IQueryable<MeasureData> createFiltered(IQueryable<MeasureData> set)
        {
            if (!string.IsNullOrEmpty(SearchString)) return set;
           return set.Where(s => s.Name.Contains(SearchString)
                                                   || s.Code.Contains(SearchString)
                                                   || s.Id.Contains(SearchString)
                                                   || s.Definition.Contains(SearchString)
                                                   || s.ValidFrom.ToString().Contains(SearchString)
                                                   || s.ValidTo.ToString().Contains(SearchString)
                                                   );
        }


        private IQueryable<MeasureData> createSorted()
        {
            IQueryable<MeasureData> measures = from s in db.Measures select s;

            switch (SortOrder)
            {
                case "name_desc":
                    measures = measures.OrderByDescending(s => s.Name);
                    break;
                case "Date":
                    measures = measures.OrderBy(s => s.ValidFrom);
                    break;
                case "date_desc":
                    measures = measures.OrderByDescending(s => s.ValidFrom);
                    break;
                default:
                    measures = measures.OrderBy(s => s.Name);
                    break;
            }

            return measures.AsNoTracking();
        }
    

        public async Task<Measure> Get(string id)
        {
            var d = await db.Measures.FirstOrDefaultAsync(m=> m.Id == id);
            return new Measure(d);
        }

        public async Task Delete(string id)
        {
            var d = await db.Measures.FindAsync(id);

            if (d is null) return;

            db.Measures.Remove(d); 
            await db.SaveChangesAsync();
        }

        public async Task Add(Measure obj)
        {
            db.Measures.Add(obj.Data);
            await db.SaveChangesAsync();
        }

        public async Task Update(Measure obj)
        {
            var d = await db.Measures.FirstOrDefaultAsync(x => x.Id == obj.Data.Id);
            d.Code = obj.Data.Code;
            d.Name = obj.Data.Name;
            d.Definition = obj.Data.Definition;
            d.ValidFrom = obj.Data.ValidFrom;
            d.ValidTo = obj.Data.ValidTo;
            db.Measures.Update(d);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //if (!(MeasureView.Id))
                //{
                //    return NotFound();
                //}
                //else
                //{
                    throw;
                //}
            }
        }
    }
}
