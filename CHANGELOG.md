# 0.3.0

## Ocelot.Windows.WindowManager

- Added the ability to add more windows via AddWindow(OcelotWindow window)
- Added HasWindow<T>, GetWindow<T>, & TryGetWindow<T>
- Auto register windows with the OcelotWindowAttribute decorator

## Ocelot.Windows.Windows

- Added

## Ocelot.Windows.[OcelotConfigWindowAttribute/OcelotMainWindowAttribute]

- Corrected namespace

# 0.3.1

## Ocelot.Modules.ModuleManager

- Ensure module order is respected when check if a module is last in the list

# 0.4.0

## Ocelot.Config.Attributes.RequiredPluginAttribute

- Added

## Ocelot.Modules.ModuleConfig

- Use RequiredPluginAttribute to ensure we only display configs where we have valid plugins

## Ocelot.OcelotUI

- Added Error(string error)
  - Prints as red message
