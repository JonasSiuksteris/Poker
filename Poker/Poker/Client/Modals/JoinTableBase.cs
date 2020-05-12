using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Poker.Client.Services;
using Poker.Shared.Models;

namespace Poker.Client.Modals
{
    public class JoinTableBase : ComponentBase
    {
        [Inject] public IAuthService AccountService { get; set; }
        [CascadingParameter] public BlazoredModalInstance BlazoredModal { get; set; }
        public JoinTableModal JoinTableModal { get; set; } = new JoinTableModal();
        public bool ShowErrors { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        protected void JoinTable()
        {
            BlazoredModal.Close(ModalResult.Ok(JoinTableModal.Amount));
        }

        protected async Task HandleSubmit()
        {
            ShowErrors = false;
            Errors = new List<string>();
            var isValid = await AccountService.GetBalance() >= JoinTableModal.Amount && JoinTableModal.Amount > 0;

            if (isValid)
            {
                JoinTable();
            }
            else
            {
                Errors.Add("Invalid amount");
                ShowErrors = true;
            }
        }
    }
}
