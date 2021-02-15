using AutoMapper;
using BasicRedisChat.BLL.Dtos;
using BasicRedisChat.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRedisChat.BLL.Helpers
{
    public static class ConvertToDto
    {
        public static IMapper Mapper = null;

        public static TDto ToDto<TDto>(this BaseEntity obj) where TDto : BaseDto
        {
            if (Mapper == null)
            {
                throw new System.Exception("Incorrect initialization for AutoMapper Helper");
            }
            return Mapper.Map<BaseEntity, TDto>(obj);
        }
    }
}
