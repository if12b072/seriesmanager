using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SeriesManager.GUI.Behavior
{
  /// <summary>
  /// Klasse kann für ein beliebiges Event ein Command ausführen
  /// </summary>
  public class EventToCommandBehavior
  {
    public static ICommand GetCommand(DependencyObject obj)
    {
      return (ICommand)obj.GetValue(CommandProperty);
    }

    public static void SetCommand(DependencyObject obj, ICommand value)
    {
      obj.SetValue(CommandProperty, value);
    }

    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(EventToCommandBehavior), new PropertyMetadata(null));



    public static string GetRoutedEvent(DependencyObject obj)
    {
      return (string)obj.GetValue(RoutedEventProperty);
    }

    public static void SetRoutedEvent(DependencyObject obj, string value)
    {
      obj.SetValue(RoutedEventProperty, value);
    }

    // Using a DependencyProperty as the backing store for RoutedEvent.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty RoutedEventProperty =
        DependencyProperty.RegisterAttached("RoutedEvent", typeof(string), typeof(EventToCommandBehavior), new PropertyMetadata(null, OnRoutedEventChanged));

    private static void OnRoutedEventChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      string eventName = e.NewValue as string;
      if (eventName == null)
        throw new ArgumentNullException("eventName");
      var eventInfo = GetEventInfo(o.GetType(), eventName);
      if (eventInfo == null)
        throw new Exception("Event " + eventName + " not found on object of type " + o.GetType());

      AttachEventHandler(o, eventInfo);
    }

    private static EventInfo GetEventInfo(Type type, string eventName)
    {
      var eventInfo = type.GetRuntimeEvent(eventName);
      if (eventInfo == null)
      {
        var baseType = type.GetTypeInfo().BaseType;
        if (baseType != null)
          return GetEventInfo(baseType, eventName);
      }
      return eventInfo;
    }

    private static void AttachEventHandler(DependencyObject o, EventInfo eventInfo)
    {
      MethodInfo methodInfo = typeof(EventToCommandBehavior).GetTypeInfo().GetDeclaredMethod("CommonEventHandler");
      Delegate handler = methodInfo.CreateDelegate(eventInfo.EventHandlerType);

      WindowsRuntimeMarshal.AddEventHandler<Delegate>(
         dlg => (EventRegistrationToken)eventInfo.AddMethod.Invoke(o, new object[] { dlg }),
          etr => eventInfo.RemoveMethod.Invoke(o, new object[] { etr }), handler);

    }
    private static void CommonEventHandler(object s, object e)
    {
      var obj = s as DependencyObject;
      ICommand command = GetCommand(obj);
      if (command != null)
        command.Execute(e);
    }
  }

}
