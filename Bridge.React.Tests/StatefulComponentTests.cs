﻿using ProductiveRage.SealedClassVerification;
using static Bridge.QUnit.QUnit;

namespace Bridge.React.Tests
{
	public static class StatefulComponentTests
	{
		public static void RunTests()
		{
			Module("(Stateful) Component Tests");

			Test("GetInitialState is called before first Render", assert =>
			{
				var done = assert.Async();
				TestComponentMounter.Render(
					new ComponentThatRendersTextFromItsState(),
					container =>
					{
						container.Remove();
						assert.StrictEqual(container.TextContent.Trim(), "Initial Text");
						done();
					}
				);
			});

			Test("GetInitialState is called on base component from derived component (that doesn't override it) before first Render", assert =>
			{
				var done = assert.Async();
				TestComponentMounter.Render(
					new DerivedFromComponentThatRendersTextFromItsState(),
					container =>
					{
						container.Remove();
						assert.StrictEqual(container.TextContent.Trim(), "Initial Text");
						done();
					}
				);
			});

			Test("GetInitialState may be called on base component from derived component that overrides it before first Render", assert =>
			{
				var done = assert.Async();
				TestComponentMounter.Render(
					new DerivedFromComponentThatRendersTextFromItsBaseStateWithAnAnnotation(),
					container =>
					{
						container.Remove();
						assert.StrictEqual(container.TextContent.Trim(), "Initial Text!");
						done();
					}
				);
			});

			Test("Components support properties and fields", assert =>
			{
				var done = assert.Async();
				TestComponentMounter.Render(
					new ComponentThatRendersTextFromFieldsAndProperties(),
					container =>
					{
						container.Remove();
						assert.StrictEqual(container.TextContent.Trim(), "Test1-Test2");
						done();
					}
				);
			});
		}

		[DesignedForInheritance]
		private class ComponentThatRendersTextFromItsState : Component<object, ComponentThatRendersTextFromItsState.State>
		{
			public ComponentThatRendersTextFromItsState() : base(null) { }
			protected override State GetInitialState()
			{
				return new State { Text = "Initial Text" };
			}
			public override ReactElement Render()
			{
				return DOM.Div(state.Text);
			}
			public sealed class State
			{
				public string Text { get; set; }
			}
		}

		private sealed class DerivedFromComponentThatRendersTextFromItsState : ComponentThatRendersTextFromItsState
		{
			public DerivedFromComponentThatRendersTextFromItsState() : base() { }
		}

		private sealed class DerivedFromComponentThatRendersTextFromItsBaseStateWithAnAnnotation : ComponentThatRendersTextFromItsState
		{
			public DerivedFromComponentThatRendersTextFromItsBaseStateWithAnAnnotation() : base() { }
			protected override State GetInitialState()
			{
				var state = base.GetInitialState();
				state.Text += "!";
				return state;
			}
		}

		private sealed class ComponentThatRendersTextFromFieldsAndProperties : Component<object, object>
		{
			private string _nameField;
			public ComponentThatRendersTextFromFieldsAndProperties() : base(null) { }
			protected override object GetInitialState()
			{
				_nameField = "Test1";
				NameProperty = "Test2";
				return null;
			}
			public string NameProperty { get; set; }
			public override ReactElement Render()
			{
				return DOM.Div(_nameField + "-" + NameProperty);
			}
		}
	}
}
