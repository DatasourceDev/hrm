using SBSResourceAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBSModel.Common
{
    public class StatusUtil
    {
        public static string Get_Record_Status(String status)
        {
            try
            {
                if (status == RecordStatus.Active)
                    return Resource.Active;
                else if (status == RecordStatus.Inactive)
                    return Resource.Inactive;
                else if (status == RecordStatus.Delete)
                    return Resource.Delete;
                else
                    return status;
            }
            catch
            {
                return status;
            }
        }

        public static string Get_Overall_Status(String status)
        {
            try
            {
                if (status == SBSWorkFlowAPI.Constants.WorkflowStatus.Cancelled)
                    return Resource.Cancelled;
                else if (status == SBSWorkFlowAPI.Constants.WorkflowStatus.Canceling)
                    return Resource.Canceling;
                else if (status == SBSWorkFlowAPI.Constants.WorkflowStatus.Rejected)
                    return Resource.Rejected;
                else if (status == SBSWorkFlowAPI.Constants.WorkflowStatus.Approved)
                    return Resource.Approved;
                else if (status == SBSWorkFlowAPI.Constants.WorkflowStatus.Cancellation_Rejected)
                   return Resource.Cancellation_Rejected;
                else if (status == SBSWorkFlowAPI.Constants.WorkflowStatus.Closed)
                    return Resource.Closed;
                else if (status == SBSWorkFlowAPI.Constants.WorkflowStatus.Pending)
                    return Resource.Pending;
                else if (status == SBSWorkFlowAPI.Constants.WorkflowStatus.Draft)
                   return Resource.Draft;
                else
                    return status;
            }
            catch
            {
                return status;
            }
        }
    }
}
