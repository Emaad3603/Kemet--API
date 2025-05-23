﻿using Kemet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;



namespace Kemet.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecifictions<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Criteria { get ; set ; } = null;
        public List<Expression<Func<T, object>>> Includes { get ; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> OrderBy { get; set; } = null;
        public Expression<Func<T, object>> OrderByDesc { get; set; } = null;
        public int Skip { get ; set; }
        public int Take { get; set; }
        public bool IsPaginationEnabled { get; set; } = false;

        public BaseSpecifications()
        {
           
        }

        public void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;

        }

        public void AddOrderByDesc(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDesc = orderByDescExpression;
        }

        public BaseSpecifications(Expression<Func<T, bool>> CritiriaExpression)
        {
            Criteria = CritiriaExpression;
        }

        public void ApplyPagination (int skip , int take)
        {
            IsPaginationEnabled = true; 
            Skip = skip;
            Take = take;


        }
    }
}
