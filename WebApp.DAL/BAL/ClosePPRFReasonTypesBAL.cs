using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.DAL;
using WebApp.DAL.Data;
using WebApp.DAL.Models.ResponseModels;

namespace WebApp.DAL.BAL
{
    
    public class ClosePPRFReasonTypesBAL
    {

        public List<ClosePPRFReasonTypes> GetActive()
        {
            var data = new List<ClosePPRFReasonTypes>();
            using (var _ClosePPRFReasonTypesDAL = new ClosePPRFReasonTypesDAL())
            {
                data.AddRange(_ClosePPRFReasonTypesDAL.List().Where(w => w.IsEnabled).Select(s => new ClosePPRFReasonTypes
                {
                    Id = s.Id,
                    Description = s.Description,
                    IsEnabled = s.IsEnabled
                }).ToList());
            }
            return data;
        }
        public List<ClosePPRFReasonTypes> GetAllRejectTypes()
        {
            var _ClosePPRFReasonTypesDAL = new ClosePPRFReasonTypesDAL();
            return _ClosePPRFReasonTypesDAL.GetRequest();
        }

        public bool Enable(ClosePPRFReasonTypes closePPRFReasonTypes)
        {
            var success = false;
            try
            {
                using (var _ClosePPRFReasonTypesDAL = new ClosePPRFReasonTypesDAL())
                {
                    _ClosePPRFReasonTypesDAL.ChangeActiveStatus(closePPRFReasonTypes.Id, true);
                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }

        public bool Disable(ClosePPRFReasonTypes closePPRFReasonTypes)
        {
            var success = false;
            try
            {
                using (var _ClosePPRFReasonTypesDAL = new ClosePPRFReasonTypesDAL())
                {
                    _ClosePPRFReasonTypesDAL.ChangeActiveStatus(closePPRFReasonTypes.Id, false);
                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }

        public ResponseObject<CreateClosePPRFReasonTypesResponse> Create(ClosePPRFReasonTypes closePPRFReasonTypes)
        {
            ResponseObject<CreateClosePPRFReasonTypesResponse> response = null;
            var Id = Guid.Empty;
            try
            {
                using (var _ClosePPRFReasonTypesDAL = new ClosePPRFReasonTypesDAL())
                {
                    Id = _ClosePPRFReasonTypesDAL.Save(closePPRFReasonTypes);
                }
                if (Id.Equals(Guid.Empty))
                    throw new Exception();
                else
                    response = new ResponseObject<CreateClosePPRFReasonTypesResponse>
                    {
                        ResponseType = "success",
                        Message = "Successfully created the frequency type."
                    };
            }
            catch (Exception ex)
            {
                response = new ResponseObject<CreateClosePPRFReasonTypesResponse>
                {
                    ResponseType = "error",
                    Message = "Something went wrong while creating the frequency type."
                };
            }
            return response;
        }

        public ResponseObject<UpdateCancelPPRFReasonTypeResponse> Update(ClosePPRFReasonTypes closePPRFReasonTypes)
        {
            var Id = Guid.Empty;
            ResponseObject<UpdateCancelPPRFReasonTypeResponse> response = null;
            try
            {
                using (var _ClosePPRFReasonTypesDAL = new ClosePPRFReasonTypesDAL())
                {
                    Id = _ClosePPRFReasonTypesDAL.Update(closePPRFReasonTypes);
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
