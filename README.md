# CoreUtilities
Technically a WPF app, but a library that implements several useful core functionality items including:
- BlurHost: Wraps a UI element in xaml and will give appearance of blurring background behind it. Has to have a UI element given to it to blur through the 'BlurBackground' property. May also need aligning which can be done through the 'OffsetX' and 'OffsetY' properties.
- A range of generic and useful converters for use in xaml UI.
- Some MVVM helper classes including 'EnumerableExtensions' for cloning a list and 'RangeObservableCollection' extending ObservableCollection to allow adding a range, rather than single values.
- Registry Service: Makes setting and getting registry values convenient and easy.
- SqLiteDatabaseService: Custom implementation of SQlite database which removes need to frequently re-setup SQlite commands.

# Getting started
- Add nuget CoreUtilities to your project
- Merge Styles.xaml resource dictionary from CoreUtilities into a resource dictionary in your app.xaml or other resource dictionary file. eg: 
	<Application.Resources>
		<ResourceDictionary>
			<ResourceDictionary.MergedDictionaries>
				<ResourceDictionary Source="/CoreUtils;component/Styles.xaml" />
			</ResourceDictionary.MergedDictionaries>
		</ResourceDictionary>
	</Application.Resources>
	
# Notes
Available converters:
- BooleanToVisibilityConverter
- VisibleIfFalseConverter
- MultiBoolAndConverter
- MultiBoolOrConverter
- StringNotEmptyVisibilityConverter
- StringWidthGetterConverter
- DateRangeFormatterConverter
- BooleanInverter
- VisibleIfOneTrueConverter
- EnumDescriptionGetterConverter
- ValueHalverConverter
- ValueDoublerConverter
- ValueSubtract2Converter
- DataGridWrapRowsBoolConverter
- PercentFormatterConverter
- IsNotNullConverter
- SequenceHasElementsVisibilityConverter
- DoubleInverterConverter