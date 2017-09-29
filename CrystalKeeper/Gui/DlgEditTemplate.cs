﻿using CrystalKeeper.Core;
using CrystalKeeper.GuiCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CrystalKeeper.Gui
{
    /// <summary>
    /// Represents a dialog that enables you to edit or create templates for
    /// all entries of a collection that uses it.
    /// </summary>
    class DlgEditTemplate
    {
        #region Members
        /// <summary>
        /// The project associated with the page.
        /// </summary>
        private Project project;

        /// <summary>
        /// The template being edited.
        /// </summary>
        private DataItem template;

        /// <summary>
        /// Contains and encapsulates gui functionality.
        /// </summary>
        private DlgEditTemplateGui gui;

        /// <summary>
        /// Stores the currently active field.
        /// </summary>
        private LstbxDataItem activeField;

        /// <summary>
        /// Fires when project data is changed.
        /// </summary>
        public event EventHandler DataNameChanged;

        /// <summary>
        /// Indicates whether non-template data is edited.
        /// </summary>
        private bool referencesInvalidated;
        #endregion

        #region Properties
        /// <summary>
        /// Stores the currently active field.
        /// </summary>
        private LstbxDataItem ActiveField
        {
            set
            {
                activeField = value;
                UpdateFieldData();
            }
            get
            {
                return activeField;
            }
        }

        /// <summary>
        /// Indicates whether non-template data is edited.
        /// </summary>
        public bool ReferencesInvalidated
        {
            get
            {
                return referencesInvalidated;
            }
            set
            {
                referencesInvalidated = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs an empty template gui.
        /// </summary>
        /// <param name="project">
        /// The project used in conjunction.
        /// </param>
        public DlgEditTemplate(Project project, DataItem template)
        {
            this.project = project;
            this.template = project.GetItemByGuid(template.guid);
            referencesInvalidated = false;
            activeField = null;
            ConstructPage();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Updates the gui fields to match the active field.
        /// </summary>
        private void UpdateFieldData()
        {
            //Displays the field name.
            gui.TxtblkFieldName.Text = (string)activeField.GetItem().GetData("name");

            //Displays the field data type.
            TemplateFieldType dataType = (TemplateFieldType)
                activeField.GetItem().GetData("dataType");

            switch (dataType)
            {
                case TemplateFieldType.EntryImages:
                    gui.CmbxDataType.SelectedItem = gui.ItemTypeEntryImages;
                    break;
                case TemplateFieldType.Text:
                    gui.CmbxDataType.SelectedItem = gui.ItemTypeText;
                    break;
                case TemplateFieldType.Min_Group:
                    gui.CmbxDataType.SelectedItem = gui.ItemTypeEntryMinGroup;
                    break;
                case TemplateFieldType.Min_Formula:
                    gui.CmbxDataType.SelectedItem = gui.ItemTypeEntryMinFormula;
                    break;
                case TemplateFieldType.Min_Locality:
                    gui.CmbxDataType.SelectedItem = gui.ItemTypeEntryMinLocality;
                    break;
                case TemplateFieldType.Min_Name:
                    gui.CmbxDataType.SelectedItem = gui.ItemTypeEntryMinName;
                    break;
                case TemplateFieldType.MoneyUSD:
                    gui.CmbxDataType.SelectedItem = gui.ItemTypeMoneyUSD;
                    break;
                case TemplateFieldType.Images:
                    gui.CmbxDataType.SelectedItem = gui.ItemTypeImages;
                    break;
                case TemplateFieldType.Hyperlink:
                    gui.CmbxDataType.SelectedItem = gui.ItemTypeHyperlink;
                    break;
            }

            //If checked, the entire field is invisible.
            gui.ChkbxFieldInvisible.IsChecked =
                (bool)activeField.GetItem().GetData("isVisible");

            //If checked, the field name is not displayed with the field.
            gui.ChkbxFieldNameInvisible.IsChecked =
                (bool)activeField.GetItem().GetData("isTitleVisible");

            //If checked, the field name is left of (not above) the field.
            gui.ChkbxFieldNameInline.IsChecked =
                (bool)activeField.GetItem().GetData("isTitleInline");

            //Sets visibility of image-specific field options.
            if (dataType == TemplateFieldType.Images)
            {
                gui.FieldImageOptions.Visibility = Visibility.Visible;
                gui.FieldImageOptions.IsEnabled = true;
            }
            else
            {
                gui.FieldImageOptions.Visibility = Visibility.Collapsed;
                gui.FieldImageOptions.IsEnabled = false;
            }

            if (activeField != null)
            {
                //For image-specific fields, sets the anchor position.
                switch ((TemplateImagePos)(int)activeField.GetItem().GetData("extraImagePos"))
                {
                    case TemplateImagePos.Above:
                        gui.CmbxItemAbove.IsSelected = true;
                        break;
                    case TemplateImagePos.Left:
                        gui.CmbxItemLeft.IsSelected = true;
                        break;
                    case TemplateImagePos.Right:
                        gui.CmbxItemRight.IsSelected = true;
                        break;
                    case TemplateImagePos.Under:
                        gui.CmbxItemUnder.IsSelected = true;
                        break;
                }

                //Sets the default number of extra images.
                gui.TxtbxNumImages.Text =
                    ((byte)activeField.GetItem().GetData("numExtraImages")).ToString();
            }

            //Shows or hides the unchangeable image field.
            if ((TemplateFieldType)ActiveField.GetItem()
                    .GetData("dataType") == TemplateFieldType.EntryImages)
            {
                gui.ItemTypeEntryImages.Visibility = Visibility.Visible;
                gui.CmbxDataType.IsEnabled = false;
            }
            else
            {
                gui.ItemTypeEntryImages.Visibility = Visibility.Collapsed;
                gui.CmbxDataType.IsEnabled = true;
            }
        }

        /// <summary>
        /// Sets the page content based on existing material.
        /// </summary>
        private void ConstructPage()
        {
            gui = new DlgEditTemplateGui();

            //Hides the entry images field by default.
            gui.ItemTypeEntryImages.Visibility = Visibility.Collapsed;

            #region Delete template
            gui.TxtblkDelete.MouseDown += (a, b) =>
            {
                var cols = project.GetTemplateCollections(template);
                MessageBoxResult result = MessageBoxResult.Yes;
                if (cols.Count > 0)
                {
                    result = MessageBox.Show("This template is used by " +
                        cols.Count + "collection(s). Deleting it will " +
                        "delete all associated collections. Are you sure?",
                        "Confirm deleting template", MessageBoxButton.YesNo);
                }
                else
                {
                    result = MessageBox.Show("You are about to delete " +
                        "this template. Are you sure?", "Confirm deleting " +
                        "template", MessageBoxButton.YesNo);
                }

                if (result == MessageBoxResult.Yes)
                {
                    //Deletes all template collections.
                    for (int i = 0; i < cols.Count; i++)
                    {
                        //Deletes all groups and their entry references.
                        var childGrps = project.GetCollectionGroupings(cols[i]);
                        for (int j = 0; j < childGrps.Count; j++)
                        {
                            //Deletes all entry references per group.
                            var grpEntryRefs = project.GetGroupingEntryRefs(childGrps[j]);
                            for (int k = 0; k < grpEntryRefs.Count; k++)
                            {
                                project.DeleteItem(grpEntryRefs[k]);
                            }

                            project.DeleteItem(childGrps[j]);
                        }

                        //Deletes all entries.
                        var childEnts = project.GetCollectionEntries(cols[i]);
                        for (int j = 0; j < childEnts.Count; j++)
                        {
                            //Deletes all entry fields per entry.
                            var entryFields = project.GetEntryFields(childEnts[j]);
                            for (int k = 0; k < entryFields.Count; k++)
                            {
                                project.DeleteItem(entryFields[k]);
                            }

                            project.DeleteItem(childEnts[j]);
                        }

                        project.DeleteItem(cols[i]);
                    }

                    project.DeleteItem(template);
                }

                gui.DialogResult = false;
                gui.Close();
            };
            #endregion

            #region Template name
            //Sets the template name.
            if (string.IsNullOrWhiteSpace((string)template.GetData("name")))
            {
                gui.TxtbxTemplateName.Text = "Untitled";
                template.SetData("name", gui.TxtbxTemplateName.Text);
            }
            else
            {
                gui.TxtbxTemplateName.Text = (string)template.GetData("name");
            }

            //Handles changes to the template name.
            gui.TxtbxTemplateName.TextChanged += TxtblkTemplateName_TextChanged;
            #endregion

            #region Center main images
            //Sets the checkbox value.
            gui.ChkbxCenterMainImages.IsChecked = (bool)template.GetData("centerImages");

            //Handles changes.
            gui.ChkbxCenterMainImages.Click += ChkbxCenterMainImages_Click;
            #endregion

            #region Number of extra images
            //Sets the default number of extra images.
            gui.TxtbxNumImages.Text =
                ((byte)template.GetData("numExtraImages")).ToString();

            gui.TxtbxNumImages.TextChanged += TxtbxNumImages_TextChanged;
            #endregion

            #region Extra image anchor position
            //Constructs the relevant ComboBoxItems.
            ComboBoxItem itemAbove = new ComboBoxItem();
            itemAbove.Content = "Above";
            ComboBoxItem itemLeft = new ComboBoxItem();
            itemLeft.Content = "Left of";
            ComboBoxItem itemRight = new ComboBoxItem();
            itemRight.Content = "Right of";
            ComboBoxItem itemUnder = new ComboBoxItem();

            itemUnder.Content = "Under";
            itemUnder.IsSelected = true;

            gui.CmbxImageAnchor.Items.Add(itemAbove);
            gui.CmbxImageAnchor.Items.Add(itemUnder);
            gui.CmbxImageAnchor.Items.Add(itemLeft);
            gui.CmbxImageAnchor.Items.Add(itemRight);

            //Sets the default image anchor position.
            switch ((TemplateImagePos)(int)template.GetData("extraImagePos"))
            {
                case TemplateImagePos.Above:
                    itemAbove.IsSelected = true;
                    break;
                case TemplateImagePos.Left:
                    itemLeft.IsSelected = true;
                    break;
                case TemplateImagePos.Right:
                    itemRight.IsSelected = true;
                    break;
                case TemplateImagePos.Under:
                    itemUnder.IsSelected = true;
                    break;
            }

            //Changes the image anchor position on the page.
            gui.CmbxImageAnchor.SelectionChanged += new SelectionChangedEventHandler((a, b) =>
            {
                if (itemAbove.IsSelected)
                {
                    template.SetData("extraImagePos", (int)TemplateImagePos.Above);
                }
                else if (itemLeft.IsSelected)
                {
                    template.SetData("extraImagePos", (int)TemplateImagePos.Left);
                }
                else if (itemRight.IsSelected)
                {
                    template.SetData("extraImagePos", (int)TemplateImagePos.Right);
                }
                else if (itemUnder.IsSelected)
                {
                    template.SetData("extraImagePos", (int)TemplateImagePos.Under);
                }
            });
            #endregion

            #region Use one column, Use two columns
            bool isTwoColumn = (bool)template.GetData("twoColumns");
            //Is enabled when the template uses only one column.
            gui.RadOneColumn.IsChecked = (!isTwoColumn);
            gui.RadOneColumn.Checked += RadOneColumn_Checked;

            //Is enabled when the template uses two columns.
            gui.RadTwoColumns.IsChecked = isTwoColumn;
            gui.RadTwoColumns.Checked += RadTwoColumns_Checked;

            //Disables/enables the columns on load.
            gui.LstbxCol2.IsEnabled = isTwoColumn;
            gui.BttnMoveLeft.IsEnabled = isTwoColumn;
            gui.BttnMoveRight.IsEnabled = isTwoColumn;
            #endregion

            #region Title font color
            //Sets the default rectangle color.
            gui.RectTitleFontColor.Fill = new SolidColorBrush(
                Color.FromRgb(
                (byte)template.GetData("headerColorR"),
                (byte)template.GetData("headerColorG"),
                (byte)template.GetData("headerColorB")));

            gui.RectTitleFontColor.MouseDown += RectTitleFontColor_MouseDown;
            #endregion

            #region Content font color
            //Sets the default rectangle color.
            gui.RectFontColor.Fill = new SolidColorBrush(
                Color.FromRgb(
                (byte)template.GetData("contentColorR"),
                (byte)template.GetData("contentColorG"),
                (byte)template.GetData("contentColorB")));

            gui.RectFontColor.MouseDown += RectFontColor_MouseDown;
            #endregion

            #region Font family
            //Dynamically populates the fonts by default.
            List<FontFamily> fonts = Fonts.SystemFontFamilies.ToList();
            for (int i = 0; i < fonts.Count; i++)
            {
                var itemFont = new ComboBoxItem();
                FontFamily itemFontFamily = fonts[i];
                itemFont.FontFamily = itemFontFamily;
                itemFont.Content = itemFontFamily.Source;
                gui.CmbxFontFamily.Items.Add(itemFont);
            }

            //Handles selecting a font.
            gui.CmbxFontFamily.SelectionChanged += CmbxFontFamily_SelectionChanged;

            //Sets the default font on load.
            for (int i = 0; i < gui.CmbxFontFamily.Items.Count; i++)
            {
                ComboBoxItem item = (ComboBoxItem)gui.CmbxFontFamily.Items.GetItemAt(i);
                if ((string)item.Content == (string)template.GetData("fontFamilies"))
                {
                    item.IsSelected = true;
                }
            }
            #endregion

            #region Field functions
            #region Refresh column order
            //Makes the column order of the data match the gui.
            var funcRefreshOrder = new Action(() =>
            {
                for (int i = 0; i < gui.LstbxCol1.Items.Count; i++)
                {
                    ((LstbxDataItem)gui.LstbxCol1.Items.GetItemAt(i))
                        .GetItem().SetData("columnOrder", i);
                }
                for (int i = 0; i < gui.LstbxCol2.Items.Count; i++)
                {
                    ((LstbxDataItem)gui.LstbxCol2.Items.GetItemAt(i))
                        .GetItem().SetData("columnOrder", i);
                }
            });
            #endregion

            #region Field, move left/right
            var funcFieldMove = new Action(() =>
            {
                if (ActiveField == null)
                {
                    return;
                }

                //Stores the template with the new column and position.
                DataItem template = project.GetTemplateItemTemplate(ActiveField.GetItem());
                DataItem newColumn;

                //Gets the new position of the field in the other column.
                if (gui.LstbxCol1.Items.Contains(ActiveField))
                {
                    newColumn = project.GetTemplateColumns(template).ElementAtOrDefault(1);
                }
                else
                {
                    newColumn = project.GetTemplateColumns(template).ElementAtOrDefault(0);
                }

                ActiveField.GetItem().SetData("refGuid", newColumn.guid);

                //Moves the item to the other column.
                if (gui.LstbxCol1.Items.Contains(ActiveField))
                {
                    gui.LstbxCol1.Items.Remove(ActiveField);
                    gui.LstbxCol2.Items.Add(ActiveField);
                }
                else if (gui.LstbxCol2.Items.Contains(ActiveField))
                {
                    gui.LstbxCol2.Items.Remove(ActiveField);
                    gui.LstbxCol1.Items.Add(ActiveField);
                }

                funcRefreshOrder();
            });
            #endregion

            #region Field, selected
            var funcFieldSelected = new Action<LstbxDataItem>((newItem) =>
            {
                ActiveField = newItem;
                var type = (TemplateFieldType)newItem.GetItem().GetData("dataType");

                //Disables the combobox when there are no interchangeable fields.
                gui.CmbxDataType.IsEnabled = !(type == TemplateFieldType.Images ||
                    type == TemplateFieldType.EntryImages ||
                    type == TemplateFieldType.MoneyUSD);

                //Hides non-interchangeable fields.
                if (type == TemplateFieldType.Hyperlink ||
                type == TemplateFieldType.Min_Formula ||
                type == TemplateFieldType.Min_Group ||
                type == TemplateFieldType.Min_Locality ||
                type == TemplateFieldType.Min_Name ||
                type == TemplateFieldType.Text)
                {
                    gui.CmbxDataType.IsEnabled = true;
                    gui.ItemTypeImages.Visibility = Visibility.Collapsed;
                    gui.ItemTypeMoneyUSD.Visibility = Visibility.Collapsed;
                }
                else
                {
                    gui.ItemTypeImages.Visibility = Visibility.Visible;
                    gui.ItemTypeMoneyUSD.Visibility = Visibility.Visible;
                }

                //Shows or hides the entry images field.
                if (type == TemplateFieldType.EntryImages)
                {
                    gui.ItemTypeEntryImages.Visibility = Visibility.Visible;
                }
                else
                {
                    gui.ItemTypeEntryImages.Visibility = Visibility.Collapsed;
                }

                //Ensures only one item is selected at once.
                if (gui.LstbxCol1.Items.Contains(ActiveField))
                {
                    gui.LstbxCol2.SelectedItem = null;
                }
                else
                {
                    gui.LstbxCol1.SelectedItem = null;
                }
            });
            #endregion
            #endregion

            #region Fields
            #region Populate fields
            //Adds every field in order for each column.
            List<DataItem> columns = project.GetTemplateColumns(template);
            for (int i = 0; i < columns.Count; i++)
            {
                //For each column, loops through each item for each item to
                //find the item matching the nth column order. Slow n^2 time.
                List<DataItem> columnFields = project.GetTemplateColumnFields(columns[i]);
                for (int j = 0; j < columnFields.Count; j++)
                {
                    for (int k = 0; k < columnFields.Count; k++)
                    {
                        LstbxDataItem item = new LstbxDataItem(columnFields[k]);

                        if (i == 0 &&
                            ((int)columnFields[k].GetData("columnOrder") == j))
                        {
                            gui.LstbxCol1.Items.Add(item);
                        }
                        else if (i == 1 &&
                            ((int)columnFields[k].GetData("columnOrder") == j))
                        {
                            gui.LstbxCol2.Items.Add(item);
                        }

                        item.Selected += new RoutedEventHandler((a, b) =>
                        {
                            funcFieldSelected(item);
                        });

                        //Allows renaming.
                        item.MouseDoubleClick += new MouseButtonEventHandler((a, b) =>
                        {
                            DlgTextbox dlg = new DlgTextbox("Rename item");
                            if (dlg.ShowDialog() == true)
                            {
                                string result = dlg.GetText();

                                if (!String.IsNullOrWhiteSpace(result))
                                {
                                    ActiveField.GetItem().SetData("name", result);
                                    ActiveField.Content = result;
                                    gui.TxtblkFieldName.Text = result;
                                }
                            }
                        });
                    }
                }
            }
            #endregion

            #region Add new field
            gui.TxtbxNewField.KeyDown += new KeyEventHandler((a, b) =>
            {
                //When enter is pressed.
                if (b.Key == Key.Enter && b.IsDown)
                {
                    //Creates a new field with the given name.
                    if (!String.IsNullOrWhiteSpace(gui.TxtbxNewField.Text))
                    {
                        //Finds the first column associated with the
                        //template and adds a field at the end of it.
                        List<DataItem> cols = project.GetTemplateColumns(template);
                        for (int i = 0; i < cols.Count; i++)
                        {
                            if ((bool)cols[i].GetData("isFirstColumn"))
                            {
                                //Sets the new field's data type.
                                TemplateFieldType newType = TemplateFieldType.Text;
                                if (gui.NewItemTypeEntryMinFormula.IsSelected)
                                {
                                    newType = TemplateFieldType.Min_Formula;
                                }
                                else if (gui.NewItemTypeEntryMinGroup.IsSelected)
                                {
                                    newType = TemplateFieldType.Min_Group;
                                }
                                else if (gui.NewItemTypeEntryMinLocality.IsSelected)
                                {
                                    newType = TemplateFieldType.Min_Locality;
                                }
                                else if (gui.NewItemTypeEntryMinName.IsSelected)
                                {
                                    newType = TemplateFieldType.Min_Name;
                                }
                                else if (gui.NewItemTypeHyperlink.IsSelected)
                                {
                                    newType = TemplateFieldType.Hyperlink;
                                }
                                else if (gui.NewItemTypeImages.IsSelected)
                                {
                                    newType = TemplateFieldType.Images;
                                }
                                else if (gui.NewItemTypeMoneyUSD.IsSelected)
                                {
                                    newType = TemplateFieldType.MoneyUSD;
                                }
                                else if (gui.NewItemTypeText.IsSelected)
                                {
                                    newType = TemplateFieldType.Text;
                                }

                                //Adds the new field to the left-hand column by default.
                                ulong newField = project.AddTemplateField(
                                    gui.TxtbxNewField.Text,
                                    cols[i].guid,
                                    newType,
                                    true, true, true,
                                    project.GetTemplateColumnFields(cols[i]).Count);

                                //Updates the GUI to match.
                                LstbxDataItem newItem = new LstbxDataItem(
                                    project.GetItemByGuid(newField));

                                gui.LstbxCol1.Items.Add(newItem);

                                newItem.Selected += new RoutedEventHandler((c, d) =>
                                {
                                    funcFieldSelected(newItem);
                                });

                                //Allows renaming.
                                newItem.MouseDoubleClick += new MouseButtonEventHandler((c, d) =>
                                {
                                    DlgTextbox dlg = new DlgTextbox("Rename item");
                                    if (dlg.ShowDialog() == true)
                                    {
                                        string result = dlg.GetText();

                                        if (!String.IsNullOrWhiteSpace(result))
                                        {
                                            ActiveField.GetItem().SetData("name", result);
                                            ActiveField.Content = result;
                                            gui.TxtblkFieldName.Text = result;
                                        }
                                    }
                                });

                                //Adds the new field to each entry in the template.
                                List<DataItem> collections = project.GetTemplateCollections(template);
                                for (int j = 0; j < collections.Count; j++)
                                {
                                    List<DataItem> entries = project.GetCollectionEntries(collections[j]);
                                    for (int k = 0; k < entries.Count; k++)
                                    {
                                        project.AddField(entries[k].guid, newField, "");
                                    }
                                }

                                newItem.IsSelected = true;
                                break;
                            }
                        }
                    }

                    gui.TxtbxNewField.Text = String.Empty;
                    funcRefreshOrder();
                }
            });
            #endregion

            #region Warn user dialog
            var funcWarnUser = new Func<bool>(() =>
            {
                List<DataItem> uses = project.GetTemplateCollections(template);
                if (uses.Count > 0)
                {
                    string collectionsUsing = String.Empty;
                    for (int i = 0; i < uses.Count; i++)
                    {
                        collectionsUsing += (string)uses[i].GetData("name");
                        if (i != uses.Count - 1)
                        {
                            collectionsUsing += ", ";
                        }
                    }

                    var result = MessageBox.Show(
                        "This template is in use. Deleting this field " +
                            "will delete all entry data associated with " +
                            "it. \n\nThe collections using this " +
                            "template are: " + collectionsUsing,
                        "Template in use",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Warning);

                    return (result == MessageBoxResult.OK);
                }

                return true;
            });
            #endregion

            #region Keyboard event handling
            gui.LstbxCol1.KeyDown += new KeyEventHandler((a, b) =>
            {
                #region Delete key pressed: Delete field
                if (b.Key == Key.Delete && b.IsDown &&
                    ActiveField != null)
                {
                    //Cannot delete the special entry images field.
                    if ((TemplateFieldType)(int)(ActiveField.GetItem().GetData("dataType")) ==
                        TemplateFieldType.EntryImages)
                    {
                        return;
                    }

                    //Warns the user and asks for confirmation.
                    if (!funcWarnUser())
                    {
                        return;
                    }

                    gui.LstbxCol1.Items.Remove(ActiveField);

                    //For each collection using this template.
                    DataItem template = project.GetTemplateItemTemplate(ActiveField.GetItem());
                    List<DataItem> cols = project.GetTemplateCollections(template);
                    for (int i = 0; i < cols.Count; i++)
                    {
                        //For each entry in the collection.
                        List<DataItem> entries = project.GetCollectionEntries(cols[i]);
                        for (int j = 0; j < entries.Count; j++)
                        {
                            //Finds all fields of each entry and removes fields
                            //that match the field removed from the template.
                            List<DataItem> entryFields = project.GetEntryFields(entries[j]);
                            for (int k = 0; k < entryFields.Count; k++)
                            {
                                DataItem field = project.GetFieldTemplateField(entryFields[k]);
                                if (ActiveField.GetItem().guid == field.guid)
                                {
                                    project.Items.Remove(entryFields[k]);
                                }
                            }
                        }
                    }

                    //Deletes the template field last.
                    project.Items.Remove(ActiveField.GetItem());

                    //Refreshes the gui.
                    funcRefreshOrder();

                    //Indicates the main display needs to be refreshed.
                    referencesInvalidated = true;
                }
                #endregion

                #region Right key pressed: Move to 2nd column
                else if (b.Key == Key.Right && b.IsDown && ActiveField != null)
                {
                    funcFieldMove();
                }
                #endregion

                #region F2 key pressed: Rename field
                else if (b.Key == Key.F2 && b.IsDown && ActiveField != null)
                {
                    //Allows renaming.
                    DlgTextbox dlg = new DlgTextbox("Rename item");
                    if (dlg.ShowDialog() == true)
                    {
                        string result = dlg.GetText();

                        if (!String.IsNullOrWhiteSpace(result))
                        {
                            ActiveField.GetItem().SetData("name", result);
                            ActiveField.Content = result;
                            gui.TxtblkFieldName.Text = result;
                        }
                    }
                }
                #endregion
            });

            gui.LstbxCol2.KeyDown += new KeyEventHandler((a, b) =>
            {
                #region Delete key pressed: Delete field
                if (b.Key == Key.Delete && b.IsDown &&
                    ActiveField != null)
                {
                    //Cannot delete the special entry images field.
                    if ((TemplateFieldType)(int)(ActiveField.GetItem().GetData("dataType")) ==
                        TemplateFieldType.EntryImages)
                    {
                        return;
                    }

                    //Warns the user and asks for confirmation.
                    if (!funcWarnUser())
                    {
                        return;
                    }

                    gui.LstbxCol2.Items.Remove(ActiveField);

                    //For each collection using this template.
                    DataItem template = project.GetTemplateItemTemplate(ActiveField.GetItem());
                    List<DataItem> cols = project.GetTemplateCollections(template);
                    for (int i = 0; i < cols.Count; i++)
                    {
                        //For each entry in the collection.
                        List<DataItem> entries = project.GetCollectionEntries(cols[i]);
                        for (int j = 0; j < entries.Count; j++)
                        {
                            //Finds all fields of each entry and removes fields
                            //that match the field removed from the template.
                            List<DataItem> entryFields = project.GetEntryFields(entries[j]);
                            for (int k = 0; k < entryFields.Count; k++)
                            {
                                DataItem field = project.GetFieldTemplateField(entryFields[k]);
                                if (ActiveField.GetItem().guid == field.guid)
                                {
                                    project.Items.Remove(entryFields[k]);
                                }
                            }
                        }
                    }

                    //Deletes the template field last.
                    project.Items.Remove(ActiveField.GetItem());

                    //Refreshes the gui.
                    funcRefreshOrder();

                    //Indicates the main display needs to be refreshed.
                    referencesInvalidated = true;
                }
                #endregion

                #region Left key pressed: Move to 1st column
                else if (b.Key == Key.Left && b.IsDown &&
                    gui.LstbxCol2.SelectedItem != null)
                {
                    funcFieldMove();
                }
                #endregion

                #region F2 key pressed: Rename field
                else if (b.Key == Key.F2 && b.IsDown && ActiveField != null)
                {
                    //Allows renaming.
                    DlgTextbox dlg = new DlgTextbox("Rename item");
                    if (dlg.ShowDialog() == true)
                    {
                        string result = dlg.GetText();

                        if (!String.IsNullOrWhiteSpace(result))
                        {
                            ActiveField.GetItem().SetData("name", result);
                            ActiveField.Content = result;
                            gui.TxtblkFieldName.Text = result;
                        }
                    }
                }
                #endregion
            });
            #endregion
            #endregion

            #region Left arrow key pressed
            gui.BttnMoveLeft.MouseDown += new MouseButtonEventHandler((a, b) =>
            {
                if (gui.LstbxCol2.SelectedItem != null)
                {
                    funcFieldMove();
                }
            });
            #endregion

            #region Right arrow key pressed
            gui.BttnMoveRight.MouseDown += new MouseButtonEventHandler((a, b) =>
            {
                if (gui.LstbxCol1.SelectedItem != null)
                {
                    funcFieldMove();
                }
            });
            #endregion

            #region Up arrow button pressed
            gui.BttnMoveUp.MouseDown += new MouseButtonEventHandler((a, b) =>
            {
                if (ActiveField != null)
                {
                    //Stores the relevant ListBox.
                    ListBox activeBox;

                    if (gui.LstbxCol1.Items.Contains(ActiveField))
                    {
                        activeBox = gui.LstbxCol1;
                    }
                    else
                    {
                        activeBox = gui.LstbxCol2;
                    }

                    int indPos = activeBox.Items.IndexOf(ActiveField);
                    if (indPos > 0)
                    {
                        LstbxDataItem otherField = ((LstbxDataItem)(activeBox
                            .Items.GetItemAt(indPos - 1)));

                        //Swaps data items.
                        DataItem dummy = ActiveField.GetItem();
                        ActiveField.SetItem(otherField.GetItem());
                        otherField.SetItem(dummy);

                        //Updates the GUI to match.
                        ActiveField.Refresh();
                        otherField.Refresh();
                        UpdateFieldData();
                        funcRefreshOrder();

                        //Selects the moved item.
                        otherField.IsSelected = true;
                    }
                }
            });
            #endregion

            #region Down arrow button pressed
            gui.BttnMoveDown.MouseDown += new MouseButtonEventHandler((a, b) =>
            {
                if (ActiveField != null)
                {
                    //Stores the relevant ListBox.
                    ListBox activeBox;

                    if (gui.LstbxCol1.Items.Contains(ActiveField))
                    {
                        activeBox = gui.LstbxCol1;
                    }
                    else
                    {
                        activeBox = gui.LstbxCol2;
                    }

                    int indPos = activeBox.Items.IndexOf(ActiveField);
                    if (indPos < activeBox.Items.Count - 1)
                    {
                        LstbxDataItem otherField = ((LstbxDataItem)(activeBox
                            .Items.GetItemAt(indPos + 1)));

                        //Swaps data items.
                        DataItem dummy = ActiveField.GetItem();
                        ActiveField.SetItem(otherField.GetItem());
                        otherField.SetItem(dummy);

                        //Updates the GUI to match.
                        ActiveField.Refresh();
                        otherField.Refresh();
                        UpdateFieldData();
                        funcRefreshOrder();

                        //Selects the moved item.
                        otherField.IsSelected = true;
                    }
                }
            });
            #endregion

            gui.CmbxDataType.SelectionChanged += CmbxDataType_SelectionChanged;
            gui.ChkbxFieldInvisible.Click += ChkbxFieldInvisible_Click;
            gui.ChkbxFieldNameInvisible.Click += ChkbxFieldNameInvisible_Click;
            gui.ChkbxFieldNameInline.Click += ChkbxFieldNameInline_Click;
            gui.CmbxFieldImageAnchor.SelectionChanged += CmbxFieldImageAnchor_SelectionChanged;
            gui.TxtbxFieldNumImages.TextChanged += TxtbxFieldNumImages_TextChanged;
            gui.BttnSaveChanges.Click += BttnSaveChanges_Click;
        }

        /// <summary>
        /// Stores the number of images for the associated image field.
        /// </summary>
        private void TxtbxFieldNumImages_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (activeField == null)
            {
                return;
            }

            //Filters non-numeric input.
            string data = String.Empty;
            for (int i = 0; i < gui.TxtbxNumImages.Text.Length; i++)
            {
                switch (gui.TxtbxNumImages.Text[i])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        data += gui.TxtbxNumImages.Text[i];
                        break;
                    default:
                        break;
                }
            }
            gui.TxtbxNumImages.Text = data;

            //Handles empty text.
            if (String.IsNullOrWhiteSpace(gui.TxtbxNumImages.Text))
            {
                gui.TxtbxNumImages.Text = "0";
            }

            //Handles copy/pasted giant numbers.
            byte testByte;
            if (!Byte.TryParse(gui.TxtbxNumImages.Text, out testByte))
            {
                gui.TxtbxNumImages.Text = "100";
            }
            //Handles numbers in the byte range above 100.
            else if (Byte.Parse(gui.TxtbxNumImages.Text) > 100)
            {
                gui.TxtbxNumImages.Text = "100";
            }

            //Sets the underlying data.
            activeField.GetItem().SetData("numExtraImages", Byte.Parse(gui.TxtbxNumImages.Text));
        }

        /// <summary>
        /// Stores the anchor position for the associated image field.
        /// </summary>
        private void CmbxFieldImageAnchor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (activeField == null)
            {
                return;
            }

            if (gui.CmbxItemAbove.IsSelected)
            {
                activeField.GetItem().SetData("extraImagePos", (int)TemplateImagePos.Above);
            }
            else if (gui.CmbxItemLeft.IsSelected)
            {
                activeField.GetItem().SetData("extraImagePos", (int)TemplateImagePos.Left);
            }
            else if (gui.CmbxItemRight.IsSelected)
            {
                activeField.GetItem().SetData("extraImagePos", (int)TemplateImagePos.Right);
            }
            else if (gui.CmbxItemUnder.IsSelected)
            {
                activeField.GetItem().SetData("extraImagePos", (int)TemplateImagePos.Under);
            }
        }

        /// <summary>
        /// Changes whether the name of the field is displayed above the field
        /// or to the left of it. If checked, the field is inline to the left.
        /// </summary>
        private void ChkbxFieldNameInline_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveField == null)
            {
                return;
            }

            activeField.GetItem().SetData("isTitleInline",
                gui.ChkbxFieldNameInline.IsChecked);
        }

        /// <summary>
        /// Changes whether the name of the field is displayed or hidden. It
        /// won't be displayed if the field is hidden.
        /// </summary>
        private void ChkbxFieldNameInvisible_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveField == null)
            {
                return;
            }

            activeField.GetItem().SetData("isTitleVisible",
                gui.ChkbxFieldNameInvisible.IsChecked);
        }

        /// <summary>
        /// Returns true and closes.
        /// </summary>
        private void BttnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            gui.DialogResult = true;
            gui.Close();
        }

        /// <summary>
        /// Changes whether a field is displayed on the page or hidden.
        /// </summary>
        private void ChkbxFieldInvisible_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveField == null)
            {
                return;
            }

            activeField.GetItem().SetData("isVisible",
                gui.ChkbxFieldInvisible.IsChecked);
        }

        /// <summary>
        /// Changes the type of data contained in a field.
        /// </summary>
        private void CmbxDataType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ActiveField == null)
            {
                return;
            }

            //Sets the desired data type.
            else if (gui.ItemTypeText.IsSelected)
            {
                activeField.GetItem().SetData("dataType",
                    (int)TemplateFieldType.Text);
            }
            else if (gui.ItemTypeMoneyUSD.IsSelected)
            {
                activeField.GetItem().SetData("dataType",
                    (int)TemplateFieldType.MoneyUSD);
            }
            else if (gui.ItemTypeImages.IsSelected)
            {
                activeField.GetItem().SetData("dataType",
                    (int)TemplateFieldType.Images);
            }
            else if (gui.ItemTypeHyperlink.IsSelected)
            {
                activeField.GetItem().SetData("dataType",
                    (int)TemplateFieldType.Hyperlink);
            }
            else if (gui.ItemTypeEntryMinFormula.IsSelected)
            {
                activeField.GetItem().SetData("dataType",
                    (int)TemplateFieldType.Min_Formula);
            }
            else if (gui.ItemTypeEntryMinName.IsSelected)
            {
                activeField.GetItem().SetData("dataType",
                    (int)TemplateFieldType.Min_Name);
            }
            else if (gui.ItemTypeEntryMinGroup.IsSelected)
            {
                activeField.GetItem().SetData("dataType",
                    (int)TemplateFieldType.Min_Group);
            }
            else if (gui.ItemTypeEntryMinLocality.IsSelected)
            {
                activeField.GetItem().SetData("dataType",
                    (int)TemplateFieldType.Min_Locality);
            }

            //Sets the field's visibility.
            gui.ChkbxFieldInvisible.IsChecked =
                (bool)activeField.GetItem().GetData("isVisible");

            //Sets the field name's visibility.
            gui.ChkbxFieldNameInvisible.IsChecked =
                (bool)activeField.GetItem().GetData("isTitleVisible");

            //Sets the field name's location relative to the field.
            gui.ChkbxFieldNameInline.IsChecked =
                (bool)activeField.GetItem().GetData("isTitleInline");

            //Sets visibility of image-specific field options.
            if (gui.ItemTypeImages.IsSelected)
            {
                gui.FieldImageOptions.Visibility = Visibility.Visible;
                gui.FieldImageOptions.IsEnabled = true;
            }
            else
            {
                gui.FieldImageOptions.Visibility = Visibility.Collapsed;
                gui.FieldImageOptions.IsEnabled = false;
            }

            if (activeField != null)
            {
                //For image-specific fields, sets the anchor position.
                switch ((TemplateImagePos)(int)activeField.GetItem().GetData("extraImagePos"))
                {
                    case TemplateImagePos.Above:
                        gui.CmbxItemAbove.IsSelected = true;
                        break;
                    case TemplateImagePos.Left:
                        gui.CmbxItemLeft.IsSelected = true;
                        break;
                    case TemplateImagePos.Right:
                        gui.CmbxItemRight.IsSelected = true;
                        break;
                    case TemplateImagePos.Under:
                        gui.CmbxItemUnder.IsSelected = true;
                        break;
                }

                //Sets the default number of extra images.
                gui.TxtbxNumImages.Text =
                    ((byte)activeField.GetItem().GetData("numExtraImages")).ToString();
            }
        }

        /// <summary>
        /// Sets the font family when the user selects it.
        /// </summary>
        private void CmbxFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ComboBoxItem)gui.CmbxFontFamily.SelectedItem;

            template.SetData("fontFamilies", (string)item.Content);
        }

        /// <summary>
        /// Opens a color dialog when the user elects to change the font color.
        /// </summary>
        private void RectFontColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var dlg = new System.Windows.Forms.ColorDialog();
            dlg.FullOpen = true;
            dlg.SolidColorOnly = true;
            var result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Yes ||
                result == System.Windows.Forms.DialogResult.OK)
            {
                template.SetData("contentColorR", dlg.Color.R);
                template.SetData("contentColorG", dlg.Color.G);
                template.SetData("contentColorB", dlg.Color.B);

                //Updates the GUI to match.
                gui.RectFontColor.Fill = new SolidColorBrush(
                    Color.FromRgb(dlg.Color.R, dlg.Color.G, dlg.Color.B));
            }
        }

        /// <summary>
        /// Opens a color dialog when the user elects to change the title
        /// font color.
        /// </summary>
        private void RectTitleFontColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var dlg = new System.Windows.Forms.ColorDialog();
            dlg.FullOpen = true;
            dlg.SolidColorOnly = true;
            var result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Yes ||
                result == System.Windows.Forms.DialogResult.OK)
            {
                template.SetData("headerColorR", dlg.Color.R);
                template.SetData("headerColorG", dlg.Color.G);
                template.SetData("headerColorB", dlg.Color.B);

                //Updates the GUI to match.
                gui.RectTitleFontColor.Fill = new SolidColorBrush(
                    Color.FromRgb(dlg.Color.R, dlg.Color.G, dlg.Color.B));
            }
        }

        /// <summary>
        /// Whether the page layout uses one or two columns. This handles when
        /// the two-column button is clicked.
        /// </summary>
        private void RadTwoColumns_Checked(object sender, RoutedEventArgs e)
        {
            template.SetData("twoColumns", true);

            //Enables column 2.
            gui.LstbxCol2.IsEnabled = true;
            gui.BttnMoveLeft.IsEnabled = true;
            gui.BttnMoveRight.IsEnabled = true;
        }

        /// <summary>
        /// Whether the page layout uses one or two columns. This handles when
        /// the one-column button is clicked.
        /// </summary>
        private void RadOneColumn_Checked(object sender, RoutedEventArgs e)
        {
            template.SetData("twoColumns", false);

            //Appends all items from column 2 to column 1.
            for (int i = 0; i < gui.LstbxCol2.Items.Count; i++)
            {
                var item = (LstbxDataItem)gui.LstbxCol2.Items.GetItemAt(i);

                //Stores the template with the new column and position.
                DataItem template = project.GetTemplateItemTemplate(item.GetItem());
                DataItem leftCol = project.GetTemplateColumns(template).ElementAt(0);

                item.GetItem().SetData("refGuid", leftCol.guid);

                //Moves the item to the other column.
                gui.LstbxCol2.Items.Remove(item);
                gui.LstbxCol1.Items.Add(item);
                i--;
            }

            //Refreshes column order.
            for (int i = 0; i < gui.LstbxCol1.Items.Count; i++)
            {
                ((LstbxDataItem)gui.LstbxCol1.Items.GetItemAt(i))
                    .GetItem().SetData("columnOrder", i);
            }

            //Disables column 2.
            gui.LstbxCol2.IsEnabled = false;
            gui.BttnMoveLeft.IsEnabled = false;
            gui.BttnMoveRight.IsEnabled = false;
        }

        /// <summary>
        /// Handles changes to the number of images.
        /// </summary>
        private void TxtbxNumImages_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Filters non-numeric input.
            string data = String.Empty;
            for (int i = 0; i < gui.TxtbxNumImages.Text.Length; i++)
            {
                switch (gui.TxtbxNumImages.Text[i])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        data += gui.TxtbxNumImages.Text[i];
                        break;
                    default:
                        break;
                }
            }
            gui.TxtbxNumImages.Text = data;

            //Handles empty text.
            if (String.IsNullOrWhiteSpace(gui.TxtbxNumImages.Text))
            {
                gui.TxtbxNumImages.Text = "0";
            }

            //Handles copy/pasted giant numbers.
            byte testByte;
            if (!Byte.TryParse(gui.TxtbxNumImages.Text, out testByte))
            {
                gui.TxtbxNumImages.Text = "100";
            }
            //Handles numbers in the byte range above 100.
            else if (Byte.Parse(gui.TxtbxNumImages.Text) > 100)
            {
                gui.TxtbxNumImages.Text = "100";
            }

            //Sets the underlying data.
            template.SetData("numExtraImages", Byte.Parse(gui.TxtbxNumImages.Text));
        }

        /// <summary>
        /// Handles changes.
        /// </summary>
        private void ChkbxCenterMainImages_Click(object sender, RoutedEventArgs e)
        {
            template.SetData("centerImages", gui.ChkbxCenterMainImages.IsChecked);
        }

        /// <summary>
        /// Handles changes to the template name.
        /// </summary>
        private void TxtblkTemplateName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(gui.TxtbxTemplateName.Text))
            {
                template.SetData("name", gui.TxtbxTemplateName.Text);
            }

            //If the textbox is empty, it will keep the last character.
            gui.TxtbxTemplateName.Text = (string)template.GetData("name");

            DataNameChanged?.Invoke(this, null);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Shows the dialog.
        /// </summary>
        public bool? ShowDialog()
        {
            return gui.ShowDialog();
        }
        #endregion
    }
}