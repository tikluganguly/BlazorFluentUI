using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BlazorFabric
{
    public class ButtonBase : FabricComponentBase
    {
        internal ButtonBase()
        {

        }

        public ElementReference ButtonRef { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public string Href { get; set; }
        [Parameter] public bool Primary { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool AllowDisabledFocus { get; set; }
        [Parameter] public bool PrimaryDisabled { get; set; }
        [Parameter] public bool? Checked { get; set; }
        //[Parameter] public string AriaLabel { get; set; }
        [Parameter] public string AriaDescripton { get; set; }
        //[Parameter] public bool AriaHidden { get; set; }
        [Parameter] public string Text { get; set; }
        [Parameter] public bool Toggle { get; set; }
        [Parameter] public bool Split { get; set; }
        [Parameter] public string IconName { get; set; }
        [Parameter] public bool HideChevron { get; set; }

        [Parameter] public IEnumerable<IContextualMenuItem> MenuItems { get; set; }
        //[Parameter] public RenderFragment ContextualMenuContent { get; set; }
        //[Parameter] public RenderFragment ContextualMenuItemsSource { get; set; }
        //[Parameter] public RenderFragment ContextualMenuItemTemplate { get; set; }

        [Parameter] public EventCallback<bool> CheckedChanged { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }
        [Parameter] public ICommand Command { get; set; }
        [Parameter] public object CommandParameter { get; set; }
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> UnknownProperties { get; set; }

        protected bool showMenu = false;

        private ICommand command;
        protected bool commandDisabled = false;

        protected bool isChecked = false;

        private bool contextMenuShown = false;

        private bool isCompoundButton = false;
        private bool isSplitButton = false;

        protected override Task OnParametersSetAsync()
        {
            showMenu = this.MenuItems != null;

            if (Command == null && command != null)
            {
                command.CanExecuteChanged -= Command_CanExecuteChanged;
                command = null;
            }
            if (Command != null && Command != command)
            {
                if (command != null)
                {
                    command.CanExecuteChanged -= Command_CanExecuteChanged;
                }
                command = Command;
                commandDisabled = !command.CanExecute(CommandParameter);
                Command.CanExecuteChanged += Command_CanExecuteChanged;
            }

            if (this.Checked.HasValue)
            {
                isChecked = this.Checked.Value;
            }

            return base.OnParametersSetAsync();
        }

        private void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            commandDisabled = !Command.CanExecute(CommandParameter);
            InvokeAsync(StateHasChanged);
        }

        protected async void ClickHandler(MouseEventArgs args)
        {
            if (Toggle)
            {
                this.isChecked = !this.isChecked;
                await this.CheckedChanged.InvokeAsync(this.isChecked);
            }
            if (!isSplitButton && MenuItems != null)
            {
                contextMenuShown = !contextMenuShown;
            }

            await OnClick.InvokeAsync(args);

            if (Command != null)
            {
                Command.Execute(CommandParameter);
            }
        }

        private void MenuClickHandler(MouseEventArgs args)
        {
            contextMenuShown = !contextMenuShown;
        }


        public void Focus()
        {

        }

        public void DismissMenu(bool isDismissed)
        {

        }

        public void OpenMenu(bool shouldFocusOnContainer, bool shouldFocusOnMount)
        {

        }

        protected void StartRoot(RenderTreeBuilder builder, string buttonClassName)
        {
            isSplitButton = (Split && OnClick.HasDelegate && MenuItems != null);
            isCompoundButton = this.GetType() == typeof(CompoundButton);
            if (isSplitButton)
            {
                AddSplit(builder, buttonClassName);
            }
            else
            {
                AddContent(builder, buttonClassName);
            }

        }

        protected void AddSplit(RenderTreeBuilder builder, string buttonClassName)
        {
            builder.OpenElement(11, "div");
            builder.AddAttribute(12, "class", $"ms-Button-splitContainer");
            if (!Disabled && !PrimaryDisabled && !commandDisabled)
            {
                builder.AddAttribute(13, "tabindex", 0);
            }

            builder.OpenElement(14, "span");
            builder.AddAttribute(15, "style", "display: flex;");

            AddContent(builder, buttonClassName);
            AddSplitButtonMenu(builder, buttonClassName);
            AddSplitButtonDivider(builder, buttonClassName);

            builder.CloseElement();
            builder.CloseElement();
        }

        protected virtual void AddContent(RenderTreeBuilder builder, string buttonClassName)
        {
            if (this.Href == null)
            {
                builder.OpenElement(21, "button");
            }
            else
            {
                builder.OpenElement(21, "a");
                builder.AddAttribute(22, "href", this.Href);

            }

            if (Primary)
            {
                buttonClassName += " ms-Button--primary";
            }
            else
            {
                buttonClassName += " ms-Button--default";
            }
            if (isSplitButton)
            {
                builder.AddAttribute(23, "class", $"ms-Button {buttonClassName} {this.ClassName} mediumFont {(Disabled || PrimaryDisabled || commandDisabled ? "is-disabled" : "")} {(isChecked ? "is-checked" : "")}");
                builder.AddAttribute(24, "disabled", (Disabled || PrimaryDisabled || commandDisabled) && !this.AllowDisabledFocus);
            }
            else
            {
                builder.AddAttribute(23, "class", $"ms-Button {buttonClassName} {this.ClassName} mediumFont{(Disabled || commandDisabled ? " is-disabled" : "")}{(isChecked ? " is-checked" : "")}{(contextMenuShown ? " is-expanded" : "")}");
                builder.AddAttribute(24, "disabled", (this.Disabled || commandDisabled) && !this.AllowDisabledFocus);
            }
            builder.AddAttribute(25, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, this.ClickHandler));
            
            builder.AddAttribute(26, "data-is-focusable", this.Disabled || PrimaryDisabled || commandDisabled || isSplitButton ? false : true);
            builder.AddAttribute(27, "style", this.Style);
            builder.AddMultipleAttributes(28, UnknownProperties);

            builder.AddElementReferenceCapture(29, (elementRef) => { RootElementReference = elementRef; });

            //if (MenuContent != null) // menu!
            //{
            //    builder.OpenElement(7, "div");
            //    builder.AddAttribute(8, "style", "display: inline-block;");

            //    builder.CloseElement();
            //}
            //skipping KeytipData component
            builder.OpenElement(30, "div");
            builder.AddAttribute(31, "class", "ms-Button-flexContainer");

            if (this.IconName != null)
            {
                builder.OpenComponent<BlazorFabric.Icon>(11);
                builder.AddAttribute(41, "ClassName", "ms-Button-icon");
                builder.AddAttribute(42, "IconName", this.IconName);
                builder.CloseComponent();
            }
            if (this.Text != null || (isCompoundButton && (this as CompoundButton).SecondaryText != null))
            {
                builder.OpenElement(51, "div");
                builder.AddAttribute(52, "class", "ms-Button-textContainer");
                builder.OpenElement(53, "div");
                builder.AddAttribute(54, "class", "ms-Button-label");
                builder.AddContent(55, this.Text ?? "");
                builder.CloseElement();
                if (isCompoundButton && (this as CompoundButton).SecondaryText != null)
                {
                    builder.OpenElement(61, "div");
                    builder.AddAttribute(62, "class", "ms-Button-description smallFont");
                    builder.AddContent(63, (this as CompoundButton).SecondaryText);
                    builder.CloseElement();
                }
                builder.CloseElement();
            }
            if (this.AriaDescripton != null)
            {
                builder.OpenElement(71, "span");
                builder.AddAttribute(72, "class", "ms-Button-screenReaderText");
                builder.AddContent(73, this.AriaDescripton);
                builder.CloseElement();
            }
            if (this.Text == null && this.ChildContent != null)
            {
                builder.AddContent(81, this.ChildContent);
            }
            if (!isSplitButton && this.MenuItems != null && !this.HideChevron)
            {
                builder.OpenComponent<BlazorFabric.Icon>(26);
                builder.AddAttribute(91, "IconName", "ChevronDown");
                builder.AddAttribute(92, "ClassName", "ms-Button-menuIcon");
                builder.CloseComponent();
            }
            if (MenuItems != null && contextMenuShown)
            {
                builder.OpenComponent<ContextualMenu>(32);
                builder.AddAttribute(101, "FabricComponentTarget", this);
                builder.AddAttribute(102, "OnDismiss", EventCallback.Factory.Create<bool>(this, (isDismissed) => 
                { 
                    contextMenuShown = false;
                }));
                builder.AddAttribute(103, "Items", MenuItems);
                builder.AddAttribute(104, "DirectionalHint", DirectionalHint.BottomLeftEdge);
                builder.CloseComponent();
            }

            builder.CloseElement();

            //if (false)
            //{
            //    //render Menu, donotlayer,  not yet made
            //}
            //if (false) // menu causes inline-block div
            //{
            //    builder.CloseElement();
            //}

            builder.CloseElement();
        }


        protected void AddSplitButtonMenu(RenderTreeBuilder builder, string buttonClassName)
        {
            if (Primary)
            {
                builder.OpenComponent<BlazorFabric.PrimaryButton>(105);
                builder.AddAttribute(106, "IconName", "ChevronDown");
                builder.AddAttribute(107, "ClassName", $"ms-Button-menuIcon{(Disabled || commandDisabled ? " is-disabled" : "")} {(isChecked ? " is-checked" : "")}{(contextMenuShown ? " is-expanded" : "")}");
                builder.AddAttribute(108, "OnClick", EventCallback.Factory.Create(this, MenuClickHandler));
                builder.AddAttribute(109, "Disabled", Disabled);
                builder.CloseComponent();
            }
            else
            {
                builder.OpenComponent<BlazorFabric.DefaultButton>(105);
                builder.AddAttribute(106, "IconName", "ChevronDown");
                builder.AddAttribute(107, "ClassName", $"ms-Button-menuIcon{(Disabled || commandDisabled ? " is-disabled" : "")} {(isChecked ? " is-checked" : "")}{(contextMenuShown ? " is-expanded" : "")}");
                builder.AddAttribute(108, "OnClick", EventCallback.Factory.Create(this, MenuClickHandler));
                builder.AddAttribute(109, "Disabled", Disabled);
                builder.CloseComponent();
            }
        }

        protected void AddSplitButtonDivider(RenderTreeBuilder builder, string buttonClassName)
        {
            builder.OpenElement(110, "span");
            if (Primary)
            {
                builder.AddAttribute(111, "class", $"ms-Button-divider ms-Button--primary{(Disabled ? " disabled" :"")}");
            }
            else
            {
                builder.AddAttribute(111, "class", $"ms-Button-divider ms-Button--default{(Disabled ? " disabled" :"")}");
            }
            builder.AddAttribute(112, "aria-hidden", true);
            builder.CloseElement();

        }




    }
}
