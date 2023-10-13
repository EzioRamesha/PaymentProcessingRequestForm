using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.DAL;
using WebApp.DAL.Models;
using WebApp.DAL.Helpers;
using WebApp.DAL.Models.ResponseModels;
using WebApp.DAL.Data;

namespace WebApp.DAL.BAL
{
    
    public class CancelPPRFReasonTypeBAL
    {

        public List<CancelPPRFReasonTypes> GetActive()
        {
            var data = new List<CancelPPRFReasonTypes>();
            using (var _CancelPPRFReasonTypeDAL = new CancelPPRFReasonTypeDAL())
            {
                data.AddRange(_CancelPPRFReasonTypeDAL.List().Where(w => w.IsEnabled).Select(s => new CancelPPRFReasonTypes
                {
                    Id = s.Id,
                    Description = s.Description,
                    IsEnabled = s.IsEnabled
                }).ToList());
            }
            return data;
        }
        public List<WebApp.DAL.Data.CancelPPRFReasonTypes> GetAllRejectTypes()
        {
            var _CancelPPRFReasonTypeDAL = new CancelPPRFReasonTypeDAL();
            return _CancelPPRFReasonTypeDAL.GetRequest();
        }

        public bool Enable(CancelPPRFReasonTypes cancelPPRFReasonTypes)
        {
            var success = false;
            try
            {
                using (var _CancelPPRFReasonTypeDAL = new CancelPPRFReasonTypeDAL())
                {
                    _CancelPPRFReasonTypeDAL.ChangeActiveStatus(cancelPPRFReasonTypes.Id, true);
                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }

        public bool Disable(CancelPPRFReasonTypes cancelPPRFReasonTypes)
        {
            var success = false;
            try
            {
                using (var _CancelPPRFReasonTypeDAL = new CancelPPRFReasonTypeDAL())
                {
                    _CancelPPRFReasonTypeDAL.ChangeActiveStatus(cancelPPRFReasonTypes.Id, false);
                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }

        public ResponseObject<CreateCancelPPRFReasonTypeResponse> Create(CancelPPRFReasonTypes cancelPPRFReasonTypes)
        {
            ResponseObject<CreateCancelPPRFReasonTypeResponse> response = null;
            var Id = Guid.Empty;
            try
            {
                using (var _CancelPPRFReasonTypeDAL = new CancelPPRFReasonTypeDAL())
                {
                    Id = _CancelPPRFReasonTypeDAL.Save(cancelPPRFReasonTypes);
                }
                if (Id.Equals(Guid.Empty))
                    throw new Exception();
                else
                    response = new ResponseObject<CreateCancelPPRFReasonTypeResponse>
                    {
                        ResponseType = "success",
                        Message = "Successfully created the frequency type."
                    };
            }
            catch (Exception ex)
            {
                response = new ResponseObject<CreateCancelPPRFReasonTypeResponse>
                {
                    ResponseType = "error",
                    Message = "Something went wrong while creating the frequency type."
                };
            }
            return response;
        }

        public ResponseObject<UpdateCancelPPRFReasonTypeResponse> Update(CancelPPRFReasonTypes cancelPPRFReasonTypes)
        {
            var Id = Guid.Empty;
            ResponseObject<UpdateCancelPPRFReasonTypeResponse> response = null;
            try
            {
                using (var _CancelPPRFReasonTypeDAL = new CancelPPRFReasonTypeDAL())
                {
                    Id = _CancelPPRFReasonTypeDAL.Update(cancelPPRFReasonTypes);
                }
                if (Id.Equals(Guid.Empty))
                    throw new Exception();
                else
                    response = new ResponseObject<UpdateCancelPPRFReasonTypeResponse>
                    {
                        ResponseType = "success",
                        Message = "Successfully updated the Reject type."
                    };
            }
            catch (Exception ex)
            {
                response = new ResponseObject<UpdateCancelPPRFReasonTypeResponse>
                {
                    ResponseType = "error",
                    Message = "Something went wrong while updating the Reject type."
                };
            }
            return response;
        }
    }
}
