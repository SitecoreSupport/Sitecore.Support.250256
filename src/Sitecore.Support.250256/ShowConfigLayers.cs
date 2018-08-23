using Sitecore;
using Sitecore.Configuration;
using Sitecore.sitecore.admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Sitecore.Support.sitecore.admin
{
  /// <summary>
  /// Admin page that shows configuration and layers details
  /// </summary>
  public class ShowConfigLayers : AdminPage
  {
    /// <summary>
    /// LayersRepeater control.
    /// </summary>
    /// <remarks>
    /// Auto-generated field.
    /// To modify move field declaration from designer file to code-behind file.
    /// </remarks>
    protected Repeater LayersRepeater;

    /// <summary>
    /// RolesList control.
    /// </summary>
    /// <remarks>
    /// Auto-generated field.
    /// To modify move field declaration from designer file to code-behind file.
    /// </remarks>
    protected ListBox RolesList;

    protected ListBox SearchList;

    /// <summary>
    /// RoleTextBox control.
    /// </summary>
    /// <remarks>
    /// Auto-generated field.
    /// To modify move field declaration from designer file to code-behind file.
    /// </remarks>
    protected TextBox RoleTextBox;

    /// <summary>
    /// AddRoleButton control.
    /// </summary>
    /// <remarks>
    /// Auto-generated field.
    /// To modify move field declaration from designer file to code-behind file.
    /// </remarks>
    protected Button AddRoleButton;

    /// <summary>
    /// ConfigPage control.
    /// </summary>
    /// <remarks>
    /// Auto-generated field.
    /// To modify move field declaration from designer file to code-behind file.
    /// </remarks>
    protected HtmlIframe ConfigPage;

    /// <summary>
    /// ConfigPageLink control.
    /// </summary>
    /// <remarks>
    /// Auto-generated field.
    /// To modify move field declaration from designer file to code-behind file.
    /// </remarks>
    protected HyperLink ConfigPageLink;

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
      base.CheckSecurity();
      if (!Page.IsPostBack)
      {
        if (!UIUtil.IsFirefox())
        {
          ConfigPage.Visible = false;
          ConfigPageLink.Visible = true;
        }
        IEnumerable<IConfigurationLayer> dataSource = new LayeredConfigurationFiles().ConfigurationLayerProviders.SelectMany((IConfigurationLayerProvider x) => x.GetLayers());
        LayersRepeater.DataSource = dataSource;
        LayersRepeater.DataBind();
        ConfigPage.Src = "/sitecore/admin/showconfig.aspx";
        ConfigPageLink.NavigateUrl = "/sitecore/admin/showconfig.aspx";
      }
      ConfigPage.Attributes.Add("onload", "javascript:setIframeHeight(document.getElementById('" + ConfigPage.ClientID + "'));");
    }

    /// <summary>
    /// Handles the ItemDataBound event of LayersRepeater control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The instance containing the event data.</param>
    protected void LayersRepeater_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
      IConfigurationLayer configurationLayer = e.Item.DataItem as IConfigurationLayer;
      if (configurationLayer != null)
      {
        CheckBox checkBox = e.Item.FindControl("LayerCheckBox") as CheckBox;
        if (checkBox != null)
        {
          checkBox.Checked = true;
        }
        BulletedList bulletedList = e.Item.FindControl("FilesList") as BulletedList;
        if (bulletedList != null)
        {
          foreach (string configurationFile in configurationLayer.GetConfigurationFiles())
          {
            bulletedList.Items.Add(configurationFile);
          }
        }
        LinkButton linkButton = e.Item.FindControl("LayerName") as LinkButton;
        if (linkButton != null)
        {
          linkButton.Text = configurationLayer.Name;
        }
      }
    }

    /// <summary>
    /// Handles the Click event of LayerLabel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The instance containing the event data.</param>
    protected void LayerLabel_OnClick(object sender, EventArgs e)
    {
      LinkButton linkButton = sender as LinkButton;
      if (linkButton != null)
      {
        BulletedList bulletedList = linkButton.Parent.FindControl("FilesList") as BulletedList;
        if (bulletedList != null)
        {
          bulletedList.Visible = !bulletedList.Visible;
        }
      }
    }

    /// <summary>
    /// Handles the CheckedChanged event of LayerCheckBox control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The instance containing the event data.</param>
    protected void LayerCheckBox_OnCheckedChanged(object sender, EventArgs e)
    {
      UpdateConfigPage();
    }

    /// <summary>
    /// Handles the SelectedIndexChanged event of RolesList control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The instance containing the event data.</param>
    protected void RolesList_OnSelectedIndexChanged(object sender, EventArgs e)
    {
      UpdateConfigPage();
    }

    protected void SearchList_OnSelectedIndexChanged(object sender, EventArgs e)
    {
      UpdateConfigPage();
    }

    /// <summary>
    /// Updates the result config page.
    /// </summary>
    protected void UpdateConfigPage()
    {
      string selectedLayers = GetSelectedLayers();
      string selectedRoles = GetSelectedRoles();
      string selectedSearch = "Lucene";
      if (SearchList.SelectedItem != null)
      {
        selectedSearch = SearchList.SelectedItem.ToString();
      }
      string text = $"/sitecore/admin/showconfig.aspx?layer={selectedLayers}&role={selectedRoles}&search={selectedSearch}";
      ConfigPage.Src = text;
      ConfigPageLink.NavigateUrl = text;
    }

    /// <summary>
    /// Get the selected layers.
    /// </summary>
    /// <returns>Pipe-separated string of selected layers.</returns>
    protected string GetSelectedLayers()
    {
      List<string> list = new List<string>();
      foreach (RepeaterItem item in LayersRepeater.Items)
      {
        if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
        {
          CheckBox checkBox = item.FindControl("LayerCheckBox") as CheckBox;
          LinkButton linkButton = item.FindControl("LayerName") as LinkButton;
          if (checkBox != null && linkButton != null && checkBox.Checked)
          {
            list.Add(linkButton.Text);
          }
        }
      }
      return string.Join("|", list);
    }

    /// <summary>
    /// Gets the selected roles.
    /// </summary>
    /// <returns>Pipe-separated string of selected roles.</returns>
    protected string GetSelectedRoles()
    {
      List<string> list = new List<string>();
      foreach (ListItem item in RolesList.Items)
      {
        if (item.Selected)
        {
          list.Add(item.Text);
        }
      }
      return string.Join("|", list);
    }

    /// <summary>
    /// Handles the Click event of AddRoleButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The instance containing the event data.</param>
    protected void AddRoleButton_OnClick(object sender, EventArgs e)
    {
      string text = RoleTextBox.Text;
      if (!string.IsNullOrWhiteSpace(text))
      {
        ListItem item = new ListItem(text)
        {
          Selected = true
        };
        RolesList.Items.Add(item);
        RolesList.Rows = RolesList.Items.Count;
        RoleTextBox.Text = string.Empty;
      }
      UpdateConfigPage();
    }
  }
}