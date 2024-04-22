# Deluxe Journal
Deluxe Journal is a Stardew Valley mod that upgrades the in-game journal, adding new features for
keeping track of the day-to-day. Create a to-do list of tasks with a variety of different
auto-completion and renewal conditions, or just jot down what you need on the notes page.

## Install
- Install [the latest version of SMAPI](https://smapi.io)
- Install [this mod from Nexus mods](https://www.nexusmods.com/stardewvalley/mods/11436)
- Unzip into the `StardewValley/Mods` folder
- Run the game using SMAPI

## Requirements
- Stardew Valley 1.6
- SMAPI version 4.0.0 or newer

## Features

### Tasks Page
The tasks page provides a checklist for you to keep track of daily tasks. Tasks can be marked as
completed by pressing the checkbox on the left-hand side, or you can choose from a variety of
different auto-completion conditions that will automatically track your progress.

At the end of each day, all completed tasks are removed unless they are given a renew period.
Choose between daily, weekly, monthly, and annual renewals to automatically reactivate tasks
after they've been completed.

#### Task Types
| Name | Description | Example |
| ---- | ----------- | -------- |
**Basic** | With no fancy bells and whistles, this task type requires manual completion. A basic task is always created when pressing "OK" instead of the "Smart Add" button when adding a new task.
**Collect** | Collect an item, with an optional count. This broadly covers any item pickups you'd like to track. | "Collect 300 wood"
**Craft** | Craft an item, with an optional count. This task focuses on items created via the crafting menu. | "Make a solar panel"
**Upgrade Tool**\*\* | Upgrade a tool at Clint's blacksmith shop. | "Upgrade my axe"
**Build**\*\* | Construct a farm building, or multiple farm buildings. | "Build a barn"
**Farm Animal**\*\* | Purchase farm animals from Marnie's Ranch. | "Buy 4 chickens"
**Gift** | Give a villager a gift, and optionally specify an item. | "Give a poppy to Penny"
**Buy**\*\* | Buy an item from a shop, with an optional count. | "Buy 48 pumpkin seeds"
**Sell**\*\* | Sell an item to a shop or overnight via the shipping bin, with an optional count. | "Sell 10 tea saplings"

\*\* *These tasks provide cost tracking, meaning the total amount to pay/gain after completing the
task(s) will show up in the money box at the bottom of the tasks page.*

#### Tips
- *Task Options*: When adding a task, the above task types can be applied automatically (simply by
	typing a name that matches the format of the desired type) or manually by opening the options
	menu. This can also be changed after task creation by pressing the body of the task.
- *Task Order*: Click and drag to reorder tasks. Completed and inactive (waiting for renewal) tasks
	will always be grouped together, however, for readability.
- *Money Box*: Pressing the "G" symbol on the money box will toggle between "total amount to pay/gain"
	and "net wealth."
- *Notifications*: If the audio cue won't cut it, you can set the **EnableVisualTaskCompleteIndicator**
	setting in the `config.json` file to "true" to enable a visual indicator.

### Notes Page
The notes page provides a section for writing down anything that's beyond the scope of a task.
Fill it with anything you want! *The gamepad cannot be used to edit the notes currently.*

## Mod Integration
There is some rudimentary support for adding custom pages and there's groundwork done for custom tasks.
If there's interest, I can add some more information on this and implement support for it.

## See Also
- [Source code](https://github.com/MolsonCAD/DeluxeJournal)
