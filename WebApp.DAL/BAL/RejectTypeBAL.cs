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
    public class RejectTypeBAL
    {

        public List<RejectType> GetActive()
        {
            var data = new List<RejectType>();
            using (var _RejectTypesDAL = new RejectTypesDAL())
            {
                data.AddRange(_RejectTypesDAL.List().Where(w => w.IsEnabled).Select(s => new RejectType
                {
                    Id = s.Id.ToString(),                 
                    Description = s.Description,
                    IsEnabled = s.IsEnabled
                }).ToList());
            }
            return data;
        }
        public List<WebApp.DAL.Data.RejectTypes> GetAllRejectTypes()
        {
            var _RejectTypesDAL = new RejectTypesDAL();            
            return _RejectTypesDAL.GetRequest(); 
        }

        public bool Enable(RejectType rejectType)
        {
            var success = false;
            try
            {
                using (var _RejectTypesDAL = new RejectTypesDAL())
                {
                    _RejectTypesDAL.ChangeActiveStatus(rejectType.Id.ToGuid(), true);
                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }

        public bool Disable(RejectType rejectType)
        {
            var success = false;
            try
            {
                using (var _RejectTypesDAL = new RejectTypesDAL())
                {
                    _RejectTypesDAL.ChangeActiveStatus(rejectType.Id.ToGuid(), false);
                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }

        public ResponseObject<CreateRejectTypeResponse> Create(RejectType rejectType)
        {
            ResponseObject<CreateRejectTypeResponse> response = null;
            var Id = Guid.Empty;
            try
            {
                using (var _RejectTypesDAL = new RejectTypesDAL())
                {
                    Id = _RejectTypesDAL.Save(rejectType);
                }
                if (Id.Equals(Guid.Empty))
                    throw new Exception();
                else
                    response = new ResponseObject<CreateRejectTypeResponse>
                    {
                        ResponseType = "success",
                        Message = "Successfully created the frequency type."
                    };
            }
            catch (Exception ex)
            {
                response = new ResponseObject<CreateRejectTypeResponse>
                {
                    ResponseType = "error",
                    Message = "Something went wrong while creating the frequency type."
                };
            }
            return response;
        }

        public ResponseObject<UpdateRejectTypeResponse> Update(RejectType rejectType)
        {
            var Id = Guid.Empty;
            ResponseObject<UpdateRejectTypeResponse> response = null;
            try
            {
                using (var _rejectTypesDAL = new RejectTypesDAL())
                {
                    Id = _rejectTypesDAL.Update(rejectType);
                }
                if (Id.Equals(Guid.Empty))
                    throw new Exception();
                else
                    response = new ResponseObject<UpdateRejectTypeResponse>
                    {
                        ResponseType = "success",
                        Message = "Successfully updated the Reject type."
                    };
            }
            catch (Exception ex)
            {
                response = new ResponseObject<UpdateRejectTypeResponse>
                {
                    ResponseType = "error",
                    Message = "Something went wrong while updating the Reject type."
                };
            }
            return response;
        }
    }
}
