﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Models
{
    public class EntityDataContext : DataMSContext
    {
        private readonly string _connection;

        public EntityDataContext(string connection)
        {
            _connection = connection;
        }

       

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connection);
            }
        }
    }
}
