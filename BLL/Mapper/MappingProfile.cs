using BLL.DTOs;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
namespace BLL.Mapper
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
             CreateMap<Account, AccountRequestDTO>().ReverseMap();
             CreateMap<Account, AccountCreateRequestDTO>().ReverseMap();
             CreateMap<Product, ProductDTO>().ReverseMap();
        }
       
    }
}
