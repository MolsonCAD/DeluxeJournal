using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

using DeluxeJournal.Framework.Task;
using DeluxeJournal.Menus.Components;
using DeluxeJournal.Task;
using DeluxeJournal.Task.Tasks;

using Period = DeluxeJournal.Task.ITask.Period;
using static StardewValley.Menus.ClickableComponent;
using static DeluxeJournal.Task.TaskParameterAttribute;

namespace DeluxeJournal.Menus
{
    /// <summary>TasksPage child menu for editing task options.</summary>
    public class TaskOptionsMenu : IClickableMenu
    {
        private const int LabelWidth = 256;
        private const int VerticalSpacing = 64;
        private const int BottomGap = 32;

        private const int ParameterTextBoxId = 1000;
        private const int ParameterIconId = 900;

        public readonly ClickableTextureComponent backButton;
        public readonly ClickableTextureComponent cancelButton;
        public readonly ClickableTextureComponent okButton;

        public readonly ClickableComponent nameTextBoxCC;
        public readonly ClickableComponent renewPeriodDropDownCC;
        public readonly ClickableComponent weekdaysDropDownCC;
        public readonly ClickableComponent daysDropDownCC;
        public readonly ClickableComponent seasonsDropDownCC;
        public readonly List<ClickableComponent> parameterTextBoxCCs;
        public readonly List<ClickableComponent> typeIcons;

        private readonly SideScrollingTextBox _nameTextBox;
        private readonly OptionsDropDown _renewPeriodDropDown;
        private readonly OptionsDropDown _weekdaysDropDown;
        private readonly OptionsDropDown _daysDropDown;
        private readonly OptionsDropDown _seasonsDropDown;
        private readonly IList<TaskParameterTextBox> _parameterTextBoxes;
        private readonly IDictionary<int, SmartIconComponent> _parameterIcons;

        private readonly ITranslationHelper _translation;
        private readonly Texture2D _textBoxTexture;
        private readonly Rectangle _fixedContentBounds;
        private readonly ITask? _task;

        private Task.TaskFactory? _taskFactory;
        private OptionsElement? _optionHeld;
        private string _selectedTaskID;
        private string _hoverText;

        public TaskOptionsMenu(ITask task, ITranslationHelper translation) : this(translation)
        {
            _task = task;
            _selectedTaskID = task.ID;
            _taskFactory = TaskRegistry.CreateFactoryInstance(task.ID);
            _taskFactory.Initialize(task, translation);

            _nameTextBox.Text = task.Name;
            _renewPeriodDropDown.selectedOption = (int)_task.RenewPeriod;
            
            if (_task.RenewPeriod != Period.Never)
            {
                _weekdaysDropDown.selectedOption = (_task.RenewDate.DayOfMonth % 7) - 1;

                if (_task.RenewPeriod != Period.Weekly)
                {
                    _daysDropDown.selectedOption = _task.RenewDate.DayOfMonth - 1;
                    _seasonsDropDown.selectedOption = _task.RenewDate.SeasonIndex;
                }
            }

            SetupParameters();
        }

        public TaskOptionsMenu(string taskName, TaskParser taskParser, ITranslationHelper translation)
            : this(translation)
        {
            _selectedTaskID = taskParser.ID;
            _taskFactory = taskParser.Factory;
            _nameTextBox.Text = taskName;

            SetupParameters();
        }

        private TaskOptionsMenu(ITranslationHelper translation) : base(0, 0, 928, 576)
        {
            xPositionOnScreen = (Game1.uiViewport.Width / 2) - (width / 2);
            yPositionOnScreen = (Game1.uiViewport.Height / 2) - (height / 2);

            _translation = translation;
            _textBoxTexture = Game1.content.Load<Texture2D>("LooseSprites\\textBox");
            _optionHeld = null;
            _taskFactory = null;
            _task = null;
            _selectedTaskID = TaskTypes.Basic;
            _hoverText = string.Empty;

            _fixedContentBounds = default;
            _fixedContentBounds.X = xPositionOnScreen + spaceToClearSideBorder + 28;
            _fixedContentBounds.Y = yPositionOnScreen + spaceToClearTopBorder + 16;
            _fixedContentBounds.Width = width - (_fixedContentBounds.X - xPositionOnScreen) * 2;

            _parameterIcons = new Dictionary<int, SmartIconComponent>();
            _parameterTextBoxes = new List<TaskParameterTextBox>();
            parameterTextBoxCCs = new List<ClickableComponent>();
            typeIcons = new List<ClickableComponent>();

            _nameTextBox = new SideScrollingTextBox(_textBoxTexture, null, Game1.smallFont, Game1.textColor)
            {
                X = _fixedContentBounds.X + LabelWidth,
                Y = _fixedContentBounds.Y,
                Width = _fixedContentBounds.Width - LabelWidth
            };

            _renewPeriodDropDown = CreateTranslatedDropDown("ui.tasks.options.renew", 0, VerticalSpacing);
            _daysDropDown = new OptionsNumericDropDown(string.Empty, 1, 28, OptionsNumericDropDown.WrapStyle.Horizontal, 7, _renewPeriodDropDown.bounds.Right + 8, _renewPeriodDropDown.bounds.Y);

            _weekdaysDropDown = CreateTranslatedDropDown(string.Empty, _renewPeriodDropDown.bounds.Width + 8, VerticalSpacing, new Dictionary<string, string>()
            {
                { "mon", Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3043") },
                { "tue", Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3044") },
                { "wed", Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3045") },
                { "thu", Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3046") },
                { "fri", Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3047") },
                { "sat", Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3048") },
                { "sun", Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3042") }
            });

            _seasonsDropDown = CreateTranslatedDropDown(string.Empty, _renewPeriodDropDown.bounds.Width + 8, VerticalSpacing, new Dictionary<string, string>()
            {
                { "spring", Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5680") },
                { "summer", Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5681") },
                { "fall", Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5682") },
                { "winter", Game1.content.LoadString("Strings\\StringsFromCSFiles:Utility.cs.5683") }
            });

            nameTextBoxCC = new ClickableComponent(new Rectangle(_nameTextBox.X, _nameTextBox.Y, _nameTextBox.Width, _nameTextBox.Height), "")
            {
                myID = 100,
                downNeighborID = 101,
                rightNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC,
                fullyImmutable = true
            };
            
            renewPeriodDropDownCC = new ClickableComponent(_renewPeriodDropDown.bounds, "")
            {
                myID = 101,
                upNeighborID = SNAP_AUTOMATIC,
                downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC
            };
            
            weekdaysDropDownCC = new ClickableComponent(_weekdaysDropDown.bounds, "")
            {
                myID = 102,
                upNeighborID = SNAP_AUTOMATIC,
                downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC
            };
            
            daysDropDownCC = new ClickableComponent(_daysDropDown.bounds, "")
            {
                myID = 103,
                upNeighborID = SNAP_AUTOMATIC,
                downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC
            };
            
            seasonsDropDownCC = new ClickableComponent(_seasonsDropDown.bounds, "")
            {
                myID = 104,
                upNeighborID = SNAP_AUTOMATIC,
                downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC
            };

            Rectangle iconBounds = new Rectangle(0, 0, 56, 56);
            int offset = 0;
            int snapId = 0;

            foreach (string id in TaskRegistry.Keys)
            {
                iconBounds.X = _fixedContentBounds.X + LabelWidth + 12 + (offset % 576);
                iconBounds.Y = _fixedContentBounds.Y + 20 + VerticalSpacing * (2 + (offset / 576));

                typeIcons.Add(new ClickableComponent(iconBounds, id)
                {
                    myID = snapId,
                    upNeighborID = SNAP_AUTOMATIC,
                    downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                    rightNeighborID = snapId == TaskRegistry.Count - 1 ? 106 : snapId + 1,
                    leftNeighborID = snapId == 0 ? 105 : snapId - 1,
                    fullyImmutable = true
                });

                offset += 64;
                snapId++;
            }

            _fixedContentBounds.Height = typeIcons.Last().bounds.Y + VerticalSpacing + 4 - _fixedContentBounds.Y;
            height = _fixedContentBounds.Bottom + BottomGap - xPositionOnScreen;

            backButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen - 64, _fixedContentBounds.Y + 12, 64, 32),
                Game1.mouseCursors,
                new Rectangle(352, 495, 12, 11),
                4f)
            {
                myID = 105,
                downNeighborID = 0,
                rightNeighborID = nameTextBoxCC.myID,
                fullyImmutable = true
            };

            cancelButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width - 12, _fixedContentBounds.Y, 64, 64),
                DeluxeJournalMod.UiTexture,
                new Rectangle(0, 80, 16, 16),
                4f)
            {
                myID = 106,
                downNeighborID = 107,
                leftNeighborID = nameTextBoxCC.myID,
                fullyImmutable = true
            };

            okButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width - 100, yPositionOnScreen + height - 4, 64, 64),
                DeluxeJournalMod.UiTexture,
                new Rectangle(32, 80, 16, 16),
                4f)
            {
                myID = 107,
                upNeighborID = CUSTOM_SNAP_BEHAVIOR,
                rightNeighborID = cancelButton.myID,
                fullyImmutable = true
            };

            Game1.playSound("shwip");

            exitFunction = OnExit;
        }

        public void RecalculateBounds()
        {
            height = _fixedContentBounds.Bottom + BottomGap - yPositionOnScreen;

            if (_taskFactory != null)
            {
                foreach (TaskParameter parameter in _taskFactory.GetParameters())
                {
                    if (!parameter.Attribute.Hidden)
                    {
                        height += VerticalSpacing;
                    }
                }
            }

            okButton.bounds.Y = yPositionOnScreen + height - 4;
        }

        public bool CanApplyChanges()
        {
            return _nameTextBox.Text.Trim().Length > 0 && _taskFactory?.IsReady() == true;
        }

        private void ApplyChanges()
        {
            string name = _nameTextBox.Text.Trim();
            string season = _seasonsDropDown.dropDownOptions[_seasonsDropDown.selectedOption];
            ITask task = _taskFactory?.Create(name) ?? new BasicTask(name);

            task.RenewPeriod = (Period)_renewPeriodDropDown.selectedOption;
            task.RenewDate = new WorldDate(1, season, (task.RenewPeriod == Period.Weekly ? _weekdaysDropDown.selectedOption : _daysDropDown.selectedOption) + 1);

            if (_task != null)
            {
                if (DeluxeJournalMod.Instance?.TaskManager is TaskManager taskManager)
                {
                    IList<ITask> tasks = taskManager.Tasks;
                    int index = tasks.IndexOf(_task);

                    task.Active = _task.Active;
                    task.Complete = _task.Complete;
                    task.Count = _task.Count;
                    task.MarkAsViewed();
                    task.SetSortingIndex(_task.GetSortingIndex());

                    if (task.Active && task.Count >= task.MaxCount)
                    {
                        task.Complete = true;
                    }

                    task.Validate();
                    tasks.RemoveAt(index);
                    tasks.Insert(index, task);
                }
            }
            else
            {
                for (IClickableMenu parent = GetParentMenu(); parent != null; parent = parent.GetParentMenu())
                {
                    if (parent is TasksPage tasksPage)
                    {
                        tasksPage.AddTask(task);
                        break;
                    }
                }
            }
        }

        public void SelectTask(string id)
        {
            _selectedTaskID = id;
            _taskFactory = TaskRegistry.CreateFactoryInstance(id);

            SetupParameters();
        }

        private void SetupParameters()
        {
            _parameterIcons.Clear();
            _parameterTextBoxes.Clear();
            parameterTextBoxCCs.Clear();

            if (_taskFactory != null)
            {
                IReadOnlyList<TaskParameter> parameters = _taskFactory.GetParameters();

                for (int i = 0; i < parameters.Count; i++)
                {
                    TaskParameter parameter = parameters[i];

                    if (parameter.Attribute.Hidden)
                    {
                        continue;
                    }

                    Rectangle textBoxBounds = new Rectangle(
                        _fixedContentBounds.X + LabelWidth,
                        _fixedContentBounds.Bottom + VerticalSpacing * i,
                        _fixedContentBounds.Width - LabelWidth,
                        _textBoxTexture.Height);
                    Rectangle iconBounds = new Rectangle(textBoxBounds.X - 60, textBoxBounds.Y - 4, 56, 56);

                    TaskParameterTextBox textBox = new TaskParameterTextBox(parameter, _taskFactory, _textBoxTexture, null, Game1.smallFont, Game1.textColor, _translation)
                    {
                        X = textBoxBounds.X,
                        Y = textBoxBounds.Y,
                        Width = textBoxBounds.Width,
                        Label = _translation.Get("ui.tasks.parameter." + parameter.Attribute.Name).Default(parameter.Attribute.Name),
                        numbersOnly = parameter.Type == typeof(int)
                    };

                    _parameterTextBoxes.Add(textBox);
                    parameterTextBoxCCs.Add(new ClickableComponent(textBoxBounds, "")
                    {
                        myID = ParameterTextBoxId + i,
                        upNeighborID = i == 0 ? CUSTOM_SNAP_BEHAVIOR : ParameterTextBoxId + i - 1,
                        downNeighborID = i == parameters.Count - 1 ? okButton.myID : ParameterTextBoxId + i + 1,
                        rightNeighborID = cancelButton.myID,
                        leftNeighborID = CUSTOM_SNAP_BEHAVIOR,
                        fullyImmutable = true
                    });

                    SmartIconFlags mask = _taskFactory.EnabledSmartIcons & parameter.Attribute.Tag switch
                    {
                        TaskParameterTag.ItemList => SmartIconFlags.Item,
                        TaskParameterTag.Building => SmartIconFlags.Building,
                        TaskParameterTag.FarmAnimalList => SmartIconFlags.Animal,
                        TaskParameterTag.NpcName => SmartIconFlags.Npc,
                        _ => SmartIconFlags.None
                    };

                    if (mask != SmartIconFlags.None)
                    {
                        _parameterIcons.Add(i, new SmartIconComponent(iconBounds, textBox.TaskParser, ParameterIconId + i, mask, 1, false));
                    }
                }
            }

            RecalculateBounds();
            populateClickableComponentList();
        }

        private OptionsDropDown CreateTranslatedDropDown(string keyPrefix, int xOffset, int yOffset)
        {
            IDictionary<string, string> options = new Dictionary<string, string>();
            string label = _translation.Get(keyPrefix);

            foreach (Translation translation in _translation.GetTranslations())
            {
                if (translation.Key.StartsWith(keyPrefix + '.'))
                {
                    options.Add(translation.Key[(keyPrefix.Length + 1)..], translation);
                }
            }

            return CreateTranslatedDropDown(label, xOffset, yOffset, options);
        }

        private OptionsDropDown CreateTranslatedDropDown(string label, int xOffset, int yOffset, IDictionary<string, string> options)
        {
            OptionsDropDown dropDown = new OptionsDropDown(label, 0, _fixedContentBounds.X + LabelWidth + xOffset, _fixedContentBounds.Y + yOffset);

            foreach (KeyValuePair<string, string> option in options)
            {
                dropDown.dropDownOptions.Add(option.Key);
                dropDown.dropDownDisplayOptions.Add(option.Value);
            }

            dropDown.bounds.Width = 0;
            dropDown.RecalculateBounds();
            dropDown.labelOffset.X = -(dropDown.bounds.Width + LabelWidth + 8);

            return dropDown;
        }

        private void OnExit()
        {
            Game1.keyboardDispatcher.Subscriber = null;

            if (Game1.options.SnappyMenus)
            {
                Game1.activeClickableMenu?.snapToDefaultClickableComponent();
            }
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            switch (direction)
            {
                case Game1.up:
                    if (oldID == okButton.myID && parameterTextBoxCCs.Count > 0)
                    {
                        currentlySnappedComponent = parameterTextBoxCCs.Last();
                    }
                    else
                    {
                        currentlySnappedComponent = GetSelectedTypeIcon(_selectedTaskID);
                    }
                    break;
                case Game1.down:
                    if (oldID >= 0 && oldID < typeIcons.Count)
                    {
                        currentlySnappedComponent = parameterTextBoxCCs.Count > 0 ? parameterTextBoxCCs.First() : okButton;
                    }
                    else
                    {
                        currentlySnappedComponent = GetSelectedTypeIcon(_selectedTaskID);
                    }
                    break;
                case Game1.left:
                    if (oldID >= ParameterTextBoxId && oldID < ParameterTextBoxId + _parameterTextBoxes.Count)
                    {
                        if (getComponentWithID(oldID - (ParameterTextBoxId - ParameterIconId)) is ClickableComponent icon)
                        {
                            currentlySnappedComponent = icon;
                        }
                        else
                        {
                            currentlySnappedComponent = backButton;
                        }
                    }
                    break;
            }

            snapCursorToCurrentSnappedComponent();
        }

        public ClickableComponent? GetSelectedTypeIcon(string name)
        {
            foreach (ClickableComponent component in typeIcons)
            {
                if (component.name == name)
                {
                    return component;
                }
            }

            return null;
        }

        public override void populateClickableComponentList()
        {
            base.populateClickableComponentList();

            foreach (var icon in _parameterIcons.Values)
            {
                allClickableComponents.AddRange(icon.GetClickableComponents());
            }
        }

        public override void snapCursorToCurrentSnappedComponent()
        {
            base.snapCursorToCurrentSnappedComponent();

            if (currentlySnappedComponent is ClickableComponent cc
                && cc.myID >= ParameterTextBoxId
                && cc.myID < ParameterTextBoxId + _parameterTextBoxes.Count)
            {
                _parameterTextBoxes[cc.myID - ParameterTextBoxId].FillWithParsedText();
            }
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = backButton;
            snapCursorToCurrentSnappedComponent();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            switch (b)
            {
                case Buttons.B:
                    currentlySnappedComponent = cancelButton;
                    snapCursorToCurrentSnappedComponent();
                    break;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (Game1.keyboardDispatcher.Subscriber is IKeyboardSubscriber keyboard)
            {
                if (keyboard is TaskParameterTextBox activeTextBox)
                {
                    activeTextBox.FillWithParsedText();
                }

                keyboard.Selected = false;
            }

            if (backButton.containsPoint(x, y))
            {
                exitThisMenu(playSound);
            }
            else if (cancelButton.containsPoint(x, y))
            {
                ExitAllChildMenus(playSound);
            }
            else if (okButton.containsPoint(x, y) && CanApplyChanges())
            {
                ApplyChangesAndExit(playSound);
            }
            else if (nameTextBoxCC.containsPoint(x, y))
            {
                _nameTextBox.SelectMe();
                _nameTextBox.Update();
            }
            else if (renewPeriodDropDownCC.containsPoint(x, y))
            {
                _renewPeriodDropDown.receiveLeftClick(x, y);
                _optionHeld = _renewPeriodDropDown;
            }
            else if (weekdaysDropDownCC.containsPoint(x, y))
            {
                _weekdaysDropDown.receiveLeftClick(x, y);
                _optionHeld = _weekdaysDropDown;
            }
            else if (seasonsDropDownCC.containsPoint(x, y))
            {
                _seasonsDropDown.receiveLeftClick(x, y);
                _optionHeld = _seasonsDropDown;
            }
            else if (daysDropDownCC.containsPoint(x, y))
            {
                _daysDropDown.receiveLeftClick(x, y);
                _optionHeld = _daysDropDown;
            }
            else
            {
                for (int i = 0; i < typeIcons.Count; i++)
                {
                    if (typeIcons[i].containsPoint(x, y))
                    {
                        SelectTask(typeIcons[i].name);
                        return;
                    }
                }

                foreach (TaskParameterTextBox textBox in _parameterTextBoxes)
                {
                    if (textBox.ContainsPoint(x, y))
                    {
                        textBox.SelectMe();
                        textBox.Update();

                        // HACK: Game1.lastCursorMotionWasMouse gets set to true after closing the
                        // on-screen keyboard, for whatever reason, preventing the user from opening
                        // another without first snapping to another component.
                        if (Game1.options.SnappyMenus && Game1.textEntry == null)
                        {
                            Game1.showTextEntry(textBox);
                        }

                        return;
                    }
                }
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            if (_optionHeld != null)
            {
                _optionHeld.leftClickHeld(x, y);
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (_optionHeld != null)
            {
                _optionHeld.leftClickReleased(x, y);
            }

            _optionHeld = null;
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.options.SnappyMenus)
            {
                applyMovementKey(key);
            }
            else if (Game1.keyboardDispatcher.Subscriber == null)
            {
                base.receiveKeyPress(key);
            }

            if (_optionHeld != null)
            {
                _optionHeld.receiveKeyPress(key);
            }

            switch (key)
            {
                case Keys.Escape:
                    exitThisMenu();
                    break;
                case Keys.Tab:
                    CycleTextBoxes();
                    break;
                case Keys.Enter:
                    if (CycleTextBoxes() && CanApplyChanges())
                    {
                        ApplyChangesAndExit();
                    }
                    break;
            }
        }

        /// <summary>Cycle through text boxes.</summary>
        /// <param name="completeParameterText">Complete the parameter text before attempting to cycle.</param>
        /// <returns><c>true</c> if the next text box was selected and <c>false</c> otherwise.</returns>
        private bool CycleTextBoxes(bool completeParameterText = true)
        {
            if (_optionHeld != null || Game1.options.SnappyMenus)
            {
                return false;
            }

            TextBox selectTextBox = _nameTextBox;

            if (_nameTextBox.Selected)
            {
                if (_parameterTextBoxes.Count > 0)
                {
                    selectTextBox = _parameterTextBoxes.First();
                }
            }
            else if (_parameterTextBoxes.Count > 0)
            {
                for (int i = 0; i < _parameterTextBoxes.Count; i++)
                {
                    if (_parameterTextBoxes[i].Selected)
                    {
                        if (completeParameterText && _parameterTextBoxes[i].FillWithParsedText())
                        {
                            return false;
                        }
                        else if (i + 1 < _parameterTextBoxes.Count)
                        {
                            selectTextBox = _parameterTextBoxes[i + 1];
                        }

                        break;
                    }
                }
            }

            if (!selectTextBox.Selected)
            {
                selectTextBox.SelectMe();
            }

            return true;
        }

        public override void applyMovementKey(int direction)
        {
            if (_optionHeld == null)
            {
                base.applyMovementKey(direction);
            }
        }

        public override void performHoverAction(int x, int y)
        {
            _hoverText = string.Empty;

            if (cancelButton.containsPoint(x, y))
            {
                _hoverText = _translation.Get("ui.tasks.new.cancelbutton.hover");
            }
            else
            {
                for (int i = 0; i < typeIcons.Count; i++)
                {
                    if (typeIcons[i].containsPoint(x, y))
                    {
                        _hoverText = _translation.Get("task." + typeIcons[i].name);
                        break;
                    }
                }

                if (_taskFactory != null && string.IsNullOrEmpty(_hoverText))
                {
                    IReadOnlyList<TaskParameter> parameters = _taskFactory.GetParameters();

                    for (int i = 0; i < parameters.Count; i++)
                    {
                        if (_parameterIcons.TryGetValue(i, out var icon) && icon.TryGetHoverText(x, y, _translation, out string hoverText))
                        {
                            _hoverText = hoverText;
                            break;
                        }
                    }
                }
            }

            backButton.tryHover(x, y, 0.2f);
            cancelButton.tryHover(x, y, 0.2f);
            okButton.tryHover(x, y);
        }

        public override void draw(SpriteBatch b)
        {
            string title = _translation.Get("ui.tasks.options");
            Period renewPeriod = (Period)_renewPeriodDropDown.selectedOption;
            int typeSectionY = _fixedContentBounds.Y + (VerticalSpacing * 2) + 24;

            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            SpriteText.drawStringWithScrollCenteredAt(b, title, xPositionOnScreen + width / 2, yPositionOnScreen + 16);

            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            drawHorizontalPartition(b, yPositionOnScreen + 212);

            DrawLabel(b, _translation.Get("ui.tasks.options.type"), typeSectionY, Game1.textColor);

            for (int i = 0; i < typeIcons.Count; i++)
            {
                TaskRegistry.GetTaskIcon(typeIcons[i].name).DrawIcon(b,
                    typeIcons[i].bounds,
                    typeIcons[i].name == _selectedTaskID ? Color.White : Color.DimGray,
                    drawShadow: true);
            }

            if (_taskFactory != null)
            {
                IReadOnlyList<TaskParameter> parameters = _taskFactory.GetParameters();
                TaskParameter parameter;
                TaskParameterTextBox parameterTextBox;

                for (int i = 0; i < parameters.Count; i++)
                {
                    parameter = parameters[i];
                    parameterTextBox = _parameterTextBoxes[i];

                    DrawLabel(b, parameterTextBox.Label, parameterTextBox.Y, parameter.IsValid() ? Game1.textColor : Color.DarkRed);
                    parameterTextBox.Draw(b);

                    if (_parameterIcons.ContainsKey(i))
                    {
                        _parameterIcons[i].Draw(b, parameter.IsValid() ? Color.White : Color.Gray * 0.8f, false, true);
                    }
                }
            }

            DrawLabel(b, _translation.Get("ui.tasks.options.name"), _nameTextBox.Y, _nameTextBox.Text.Trim().Length > 0 ? Game1.textColor : Color.DarkRed);
            _nameTextBox.Draw(b);

            backButton.draw(b);
            cancelButton.draw(b);
            okButton.draw(b, CanApplyChanges() ? Color.White : Color.Gray * 0.8f, 0.88f);

            weekdaysDropDownCC.visible = renewPeriod == Period.Weekly;
            seasonsDropDownCC.visible = renewPeriod == Period.Annually;
            daysDropDownCC.visible = renewPeriod == Period.Monthly || renewPeriod == Period.Annually;
            daysDropDownCC.bounds.X = (renewPeriod == Period.Annually ? _seasonsDropDown.bounds.Right : _renewPeriodDropDown.bounds.Right) + 8;
            _daysDropDown.bounds.X = daysDropDownCC.bounds.X;
            _daysDropDown.dropDownBounds.X = daysDropDownCC.bounds.X;

            _renewPeriodDropDown.draw(b, 0, 0);

            if (weekdaysDropDownCC.visible)
            {
                _weekdaysDropDown.draw(b, 0, 0);
            }
            if (seasonsDropDownCC.visible)
            {
                _seasonsDropDown.draw(b, 0, 0);
            }
            if (daysDropDownCC.visible)
            {
                _daysDropDown.draw(b, 0, 0);
            }

            if (_hoverText.Length > 0)
            {
                drawHoverText(b, _hoverText, Game1.dialogueFont);
            }
        }

        private void DrawLabel(SpriteBatch b, string name, int yPos, Color color)
        {
            Utility.drawTextWithShadow(b, name, Game1.dialogueFont, new Vector2(_fixedContentBounds.X, yPos), color);
        }

        private void ApplyChangesAndExit(bool playSound = true)
        {
            ApplyChanges();

            if (playSound)
            {
                Game1.playSound("bigSelect");
            }

            ExitAllChildMenus(false);
        }

        private void ExitAllChildMenus(bool playSound = true)
        {
            if (playSound)
            {
                Game1.playSound("bigDeSelect");
            }

            for (IClickableMenu parent = GetParentMenu(); parent != null; parent = parent.GetParentMenu())
            {
                parent.GetChildMenu().exitThisMenuNoSound();
            }
        }
    }
}
