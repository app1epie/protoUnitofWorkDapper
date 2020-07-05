using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace protoUnitofWorkDapper.Models
{
    public enum MemberStatus
    {
        Deactivated = 0,
        New = 1,
        Freeze = 2
    }

    [Table("Members")]
    public class Member
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public MemberStatus Status { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class PagedResults<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int TotalCount { get; set; }
    }

    /* Paged result: https://www.davepaquette.com/archive/2019/01/28/paging-large-result-sets-with-dapper-and-sql-server.aspx
     */

    public class MemberRepository
    {
        private readonly IUnitOfWork unitOfWork;

        public MemberRepository(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public List<Member> Get()
        {
            return unitOfWork.Connection.GetAll<Member>()
                .ToList();
        }

        public PagedResults<Member> GetPaged(int page, int size)
        {
            var result = new PagedResults<Member>();

            string query = "SELECT Count(1) as 'TotalCount' FROM MEMBERS;" +
                "SELECT * FROM MEMBERS ORDER BY ID OFFSET @page ROWS FETCH NEXT @size ROWS ONLY;";

            using (var multi = unitOfWork.Connection.QueryMultiple(query, new { page, size }))
            {
                result.TotalCount = multi.Read<int>().FirstOrDefault();
                result.Data = multi.Read<Member>().ToList();
            }

            return result;
        }

        public Member Get(int id)
        {
            return unitOfWork.Connection.Get<Member>(id);
        }

        public long Insert(Member member)
        {
            return unitOfWork.Connection.Insert(member, unitOfWork.Transaction);
        }

        public bool Update(Member member)
        {
            return unitOfWork.Connection.Update(member, unitOfWork.Transaction);
        }

        public bool Deactive(int id)
        {
            return unitOfWork.Connection.Update(
                new Member { Id = id, Status = MemberStatus.Deactivated },
                unitOfWork.Transaction);
        }

        public bool Delete(Member member)
        {
            return unitOfWork.Connection.Delete(member, unitOfWork.Transaction);
        }
    }
}
