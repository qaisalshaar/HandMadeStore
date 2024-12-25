﻿using HandMadeStore.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandMadeStore.DataAccess.Repository.IRepository
{
    public interface IOrderDetailRepositry : IRepository<OrderDetail>

    {
        void Update(OrderDetail orderdetail);

       



    }
}
