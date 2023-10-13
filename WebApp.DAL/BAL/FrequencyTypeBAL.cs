using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.DAL;
using WebApp.DAL.Models;
using WebApp.DAL.Helpers;
using WebApp.DAL.Models.ResponseModels;

namespace WebApp.DAL.BAL
{
    public class FrequencyTypeBAL
    {
        //private static readonly FrequencyTypeDAL _frequencyTypeDAL = new FrequencyTypeDAL();
        public List<FrequencyType> GetActive()
        {
            var data = new List<FrequencyType>();
            using (var _frequencyTypeDAL = new FrequencyTypeDAL())
            {
                data.AddRange(_frequencyTypeDAL.List().Where(w => w.IsEnabled).Select(s => new FrequencyType
                {
                    Id = s.Id.ToString(),
                    Name = s.Name,
                    Description = s.Description,
                    IsEnabled = s.IsEnabled
                }).ToList());
            }
            return data;
        }

        public List<FrequencyType> GetAll()
        {
            var data = new List<FrequencyType>();
            using (var _frequencyTypeDAL = new FrequencyTypeDAL())
            {
                data.AddRange(_frequencyTypeDAL.List().Select(s => new FrequencyType
                {
                    Id = s.Id.ToString(),
                    Name = s.Name,
                    Description = s.Description,
                    IsEnabled = s.IsEnabled
                }).ToList());
            }
            return data;
        }

        public bool Enable(FrequencyType frequencyType)
        {
            var success = false;
            try
            {
                using (var _frequencyTypeDAL = new FrequencyTypeDAL())
                {
                    _frequencyTypeDAL.ChangeActiveStatus(frequencyType.Id.ToGuid(), true);
                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }

        public bool Disable(FrequencyType frequencyType)
        {
            var success = false;
            try
            {
                using (var _frequencyTypeDAL = new FrequencyTypeDAL())
                {
                    _frequencyTypeDAL.ChangeActiveStatus(frequencyType.Id.ToGuid(), false);
                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }


        public ResponseObject<CreateFrequencyTypeResponse> Create(FrequencyType frequencyType)
        {
            ResponseObject<CreateFrequencyTypeResponse> response = null;
            var Id = Guid.Empty;
            try
            {
                using (var _frequencyTypeDAL = new FrequencyTypeDAL())
                {
                    Id = _frequencyTypeDAL.Save(frequencyType);
                }
                if (Id.Equals(Guid.Empty))
                    throw new Exception();
                else
                    response = new ResponseObject<CreateFrequencyTypeResponse>
                    {
                        ResponseType = "success",
                        Message = "Successfully created the frequency type."
                    };
            }
            catch (Exception ex)
            {
                response = new ResponseObject<CreateFrequencyTypeResponse>
                {
                    ResponseType = "error",
                    Message = "Something went wrong while creating the frequency type."
                };
            }
            return response;
        }


        public ResponseObject<UpdateFrequencyTypeResponse> Update(FrequencyType frequencyType)
        {
            var Id = Guid.Empty;
            ResponseObject<UpdateFrequencyTypeResponse> response = null;
            try
            {
                using (var _frequencyTypeDAL = new FrequencyTypeDAL())
                {
                    Id = _frequencyTypeDAL.Update(frequencyType);
                }
                if (Id.Equals(Guid.Empty))
                    throw new Exception();
                else
                    response = new ResponseObject<UpdateFrequencyTypeResponse>
                    {
                        ResponseType = "success",
                        Message = "Successfully updated the frequency type."
                    };
            }
            catch (Exception ex)
            {
                response = new ResponseObject<UpdateFrequencyTypeResponse>
                {
                    ResponseType = "error",
                    Message = "Something went wrong while updating the frequency type."
                };
            }
            return response;
        }
    }
}
