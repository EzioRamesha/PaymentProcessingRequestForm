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
    public class PaymentMethodBAL
    {
        private static PaymentMethodDAL _paymentMethodDAL = new PaymentMethodDAL();
        public List<PaymentMethod> GetActive()
        {
            return _paymentMethodDAL.List().Where(w => w.IsEnabled).Select(s => new PaymentMethod
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Description = s.Description,
                IsEnabled = s.IsEnabled
            }).ToList();
        }

        public List<PaymentMethod> GetAll()
        {
            return _paymentMethodDAL.List().Select(s => new PaymentMethod
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Description = s.Description,
                IsEnabled = s.IsEnabled
            }).ToList();
        }

        public bool Enable(PaymentMethod paymentMethod)
        {
            var success = false;
            try
            {
                using (var _paymentMethodDAL = new PaymentMethodDAL())
                {
                    _paymentMethodDAL.ChangeActiveStatus(paymentMethod.Id.ToGuid(), true);
                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }

        public bool Disable(PaymentMethod paymentMethod)
        {
            var success = false;
            try
            {
                using (var _paymentMethodDAL = new PaymentMethodDAL())
                {
                    _paymentMethodDAL.ChangeActiveStatus(paymentMethod.Id.ToGuid(), false);
                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }


        public ResponseObject<CreatePaymentMethodResponse> Create(PaymentMethod paymentMethod)
        {
            ResponseObject<CreatePaymentMethodResponse> response = null;
            var Id = Guid.Empty;
            try
            {
                using (var _paymentMethodDAL = new PaymentMethodDAL())
                {
                    Id = _paymentMethodDAL.Save(paymentMethod);
                }
                if (Id.Equals(Guid.Empty))
                    throw new Exception();
                else
                    response = new ResponseObject<CreatePaymentMethodResponse>
                    {
                        ResponseType = "success",
                        Message = "Successfully created the payment method."
                    };
            }
            catch (Exception ex)
            {
                response = new ResponseObject<CreatePaymentMethodResponse>
                {
                    ResponseType = "error",
                    Message = "Something went wrong while updating the payment method."
                };
            }
            return response;
        }

        public ResponseObject<UpdatePaymentMethodResponse> Update(PaymentMethod paymentMethod)
        {
            var Id = Guid.Empty;
            ResponseObject<UpdatePaymentMethodResponse> response = null;
            try
            {
                using (var _paymentMethodDAL = new PaymentMethodDAL())
                {
                    Id = _paymentMethodDAL.Update(paymentMethod);
                }
                if (Id.Equals(Guid.Empty))
                    throw new Exception();
                else
                    response = new ResponseObject<UpdatePaymentMethodResponse>
                    {
                        ResponseType = "success",
                        Message = "Successfully updated the payment method."
                    };
            }
            catch (Exception ex)
            {
                response = new ResponseObject<UpdatePaymentMethodResponse>
                {
                    ResponseType = "error",
                    Message = "Something went wrong while updating the payment method."
                };
            }
            return response;
        }
    }
}
