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
    public class ExpenseTypeBAL
    {
        private static readonly ExpenseTypeDAL _expenseTypeDAL = new ExpenseTypeDAL();
        public List<ExpenseType> GetActive()
        {
            return _expenseTypeDAL.List().Where(w => w.IsEnabled).Select(s => new ExpenseType
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Description = s.Description,
                IsEnabled = s.IsEnabled
            }).ToList();
        }

        public List<ExpenseType> GetAll()
        {
            return _expenseTypeDAL.List().Select(s => new ExpenseType
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Description = s.Description,
                IsEnabled = s.IsEnabled
            }).ToList();
        }


        public bool Enable(ExpenseType expenseType)
        {
            var success = false;
            try
            {
                using (var _expenseTypeDAL = new ExpenseTypeDAL())
                {
                    _expenseTypeDAL.ChangeActiveStatus(expenseType.Id.ToGuid(), true);
                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }

        public bool Disable(ExpenseType expenseType)
        {
            var success = false;
            try
            {
                using (var _expenseTypeDAL = new ExpenseTypeDAL())
                {
                    _expenseTypeDAL.ChangeActiveStatus(expenseType.Id.ToGuid(), false);
                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }


        public ResponseObject<CreateExpenseTypeResponse> Create(ExpenseType expenseType)
        {
            ResponseObject<CreateExpenseTypeResponse> response = null;
            var Id = Guid.Empty;
            try
            {
                using (var _expenseTypeDAL = new ExpenseTypeDAL())
                {
                    Id = _expenseTypeDAL.Save(expenseType);
                }
                if (Id.Equals(Guid.Empty))
                    throw new Exception();
                else
                    response = new ResponseObject<CreateExpenseTypeResponse>
                    {
                        ResponseType = "success",
                        Message = "Successfully created the expense type."
                    };
            }
            catch (Exception ex)
            {
                response = new ResponseObject<CreateExpenseTypeResponse>
                {
                    ResponseType = "error",
                    Message = "Something went wrong while creating the expense type."
                };
            }
            return response;
        }

        public ResponseObject<UpdateExpenseTypeResponse> Update(ExpenseType expenseType)
        {
            var Id = Guid.Empty;
            ResponseObject<UpdateExpenseTypeResponse> response = null;
            try
            {
                using (var _expenseTypeDAL = new ExpenseTypeDAL())
                {
                    Id = _expenseTypeDAL.Update(expenseType);
                }
                if (Id.Equals(Guid.Empty))
                    throw new Exception();
                else
                    response = new ResponseObject<UpdateExpenseTypeResponse>
                    {
                        ResponseType = "success",
                        Message = "Successfully updated the expense type."
                    };
            }
            catch (Exception ex)
            {
                response = new ResponseObject<UpdateExpenseTypeResponse>
                {
                    ResponseType = "error",
                    Message = "Something went wrong while updating the expense type."
                };
            }
            return response;
        }
    }
}
