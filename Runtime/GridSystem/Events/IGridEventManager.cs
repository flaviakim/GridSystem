// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace GridSystem.Events {
//     public interface IGridEventManager<TGridNode> where TGridNode : IGridNode<TGridNode> {
//         void Subscribe<TEvent>(System.Action<TEvent> handler) where TEvent : GridEvent;
//         void Unsubscribe<TEvent>(System.Action<TEvent> handler) where TEvent : GridEvent;
//         void PublishEvent<TEvent>(TEvent gridEvent) where TEvent : GridEvent;
//         void PublishEventDeferred<TEvent>(TEvent gridEvent) where TEvent : GridEvent;
//         void ProcessDeferredEvents();
//         void Clear();
//     }
//     
//     public class GridEventManager<TGridNode> : IGridEventManager<TGridNode> where TGridNode : IGridNode<TGridNode> {
//         private readonly Dictionary<Type, List<object>> _eventHandlers = new();
//         private readonly Queue<GridEvent> _deferredEvents = new();
//         private readonly List<GridEvent> _eventHistory = new();
//         
//         public bool EnableEventHistory { get; set; } = false;
//         public int MaxHistorySize { get; set; } = 1000;
//
//         public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : GridEvent {
//             Type eventType = typeof(TEvent);
//             if (!_eventHandlers.ContainsKey(eventType)) {
//                 _eventHandlers[eventType] = new List<object>();
//             }
//             _eventHandlers[eventType].Add(handler);
//         }
//
//         public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : GridEvent {
//             Type eventType = typeof(TEvent);
//             if (!_eventHandlers.TryGetValue(eventType, out List<object> handlers)) return;
//             handlers.Remove(handler);
//             if (handlers.Count == 0) {
//                 _eventHandlers.Remove(eventType);
//             }
//         }
//
//         public void PublishEvent<TEvent>(TEvent gridEvent) where TEvent : GridEvent {
//             if (EnableEventHistory) {
//                 AddToHistory(gridEvent);
//             }
//
//             Type eventType = typeof(TEvent);
//             if (!_eventHandlers.TryGetValue(eventType, out List<object> handlers)) return;
//             // Create a copy to avoid issues if handlers are modified during iteration
//             var handlersCopy = new List<object>(handlers);
//             foreach (Action<TEvent> handler in handlersCopy) {
//                 try {
//                     handler?.Invoke(gridEvent);
//                 } catch (Exception ex) {
//                     Debug.LogError($"Error in grid event handler: {ex.Message}\n{ex.StackTrace}");
//                 }
//             }
//         }
//
//         public void PublishEventDeferred<TEvent>(TEvent gridEvent) where TEvent : GridEvent {
//             _deferredEvents.Enqueue(gridEvent);
//         }
//
//         public void ProcessDeferredEvents() {
//             while (_deferredEvents.Count > 0) {
//                 var eventToProcess = _deferredEvents.Dequeue();
//                 PublishDeferredEventInternal(eventToProcess);
//             }
//         }
//
//         private void PublishDeferredEventInternal(GridEvent gridEvent) {
//             if (EnableEventHistory) {
//                 AddToHistory(gridEvent);
//             }
//
//             var eventType = gridEvent.GetType();
//             if (_eventHandlers.TryGetValue(eventType, out var handlers)) {
//                 var handlersCopy = new List<object>(handlers);
//                 foreach (var handlerObj in handlersCopy) {
//                     try {
//                         // Use dynamic dispatch instead of reflection
//                         ((dynamic)handlerObj)((dynamic)gridEvent);
//                     } catch (Exception ex) {
//                         Debug.LogError($"Error in grid event handler: {ex.Message}\n{ex.StackTrace}");
//                     }
//                 }
//             }
//         }
//
//         public void Clear() {
//             _eventHandlers.Clear();
//             _deferredEvents.Clear();
//             _eventHistory.Clear();
//         }
//
//         private void AddToHistory(GridEvent gridEvent) {
//             _eventHistory.Add(gridEvent);
//             if (_eventHistory.Count > MaxHistorySize) {
//                 _eventHistory.RemoveAt(0);
//             }
//         }
//
//         public IReadOnlyList<GridEvent> GetEventHistory() {
//             return _eventHistory.AsReadOnly();
//         }
//     }
// }