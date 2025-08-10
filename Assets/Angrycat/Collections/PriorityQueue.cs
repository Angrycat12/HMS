using System;
using System.Collections.Generic;

namespace Angrycat.Collections
{
    /// <summary>
    /// Представляет коллекцию элементов, имеющих значение и приоритет. 
    /// При извлечении элемент с наименьшим значением приоритета извлекается первым.
    /// Реализация основана на двоичной минимальной куче (Binary Min-Heap).
    /// </summary>
    /// <typeparam name="TElement">Тип элементов в очереди.</typeparam>
    /// <typeparam name="TPriority">Тип, используемый для определения приоритета. Должен реализовывать IComparable.</typeparam>
    public class PriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
    {
        // Внутреннее хранилище для кучи в виде списка кортежей (элемент, приоритет).
        private readonly List<(TElement Element, TPriority Priority)> _heap;

        /// <summary>
        /// Получает количество элементов, содержащихся в очереди.
        /// </summary>
        public int Count => _heap.Count;

        /// <summary>
        /// Инициализирует новый пустой экземпляр класса PriorityQueue.
        /// </summary>
        public PriorityQueue()
        {
            _heap = new List<(TElement, TPriority)>();
        }
        
        /// <summary>
        /// Инициализирует новый экземпляр класса PriorityQueue с указанной начальной ёмкостью.
        /// </summary>
        /// <param name="initialCapacity">Начальная ёмкость очереди.</param>
        public PriorityQueue(int initialCapacity)
        {
            _heap = new List<(TElement, TPriority)>(initialCapacity);
        }

        /// <summary>
        /// Добавляет элемент в очередь с указанным приоритетом.
        /// Сложность: O(log N).
        /// </summary>
        /// <param name="element">Элемент для добавления.</param>
        /// <param name="priority">Приоритет элемента.</param>
        public void Enqueue(TElement element, TPriority priority)
        {
            // Добавляем элемент в конец кучи
            _heap.Add((element, priority));
            // Восстанавливаем свойство кучи, "просеивая" новый элемент вверх.
            SiftUp(_heap.Count - 1);
        }

        /// <summary>
        /// Удаляет и возвращает элемент с наименьшим приоритетом из очереди.
        /// Сложность: O(log N).
        /// </summary>
        /// <returns>Элемент с наименьшим приоритетом.</returns>
        /// <exception cref="InvalidOperationException">Вызывается, если очередь пуста.</exception>
        public TElement Dequeue()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Очередь пуста.");
            }

            // Элемент с наивысшим приоритетом (наименьшим значением) всегда в корне кучи.
            var elementToReturn = _heap[0].Element;
            int lastIndex = _heap.Count - 1;

            // Перемещаем последний элемент в корень.
            _heap[0] = _heap[lastIndex];
            _heap.RemoveAt(lastIndex);

            // Если в куче остались элементы, восстанавливаем свойство кучи,
            // "просеивая" новый корневой элемент вниз.
            if (Count > 0)
            {
                SiftDown(0);
            }

            return elementToReturn;
        }

        /// <summary>
        /// Возвращает элемент с наименьшим приоритетом без его удаления из очереди.
        /// Сложность: O(1).
        /// </summary>
        /// <returns>Элемент с наименьшим приоритетом.</returns>
        /// <exception cref="InvalidOperationException">Вызывается, если очередь пуста.</exception>
        public TElement Peek()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Очередь пуста.");
            }
            return _heap[0].Element;
        }

        /// <summary>
        /// Удаляет все элементы из очереди.
        /// </summary>
        public void Clear()
        {
            _heap.Clear();
        }

        /// <summary>
        /// Процедура восстановления свойства кучи путем перемещения элемента вверх.
        /// Используется после добавления нового элемента.
        /// </summary>
        private void SiftUp(int index)
        {
            if (index == 0) return;

            int parentIndex = (index - 1) / 2;

            // Если приоритет дочернего элемента меньше родительского, меняем их местами.
            if (_heap[index].Priority.CompareTo(_heap[parentIndex].Priority) < 0)
            {
                Swap(index, parentIndex);
                // Продолжаем просеивание вверх от новой позиции родителя.
                SiftUp(parentIndex);
            }
        }

        /// <summary>
        /// Процедура восстановления свойства кучи путем перемещения элемента вниз.
        /// Используется после извлечения корневого элемента.
        /// </summary>
        private void SiftDown(int index)
        {
            int leftChildIndex = 2 * index + 1;
            int rightChildIndex = 2 * index + 2;
            int smallestChildIndex = index;

            // Ищем наименьший элемент среди родителя и его дочерних элементов.
            if (leftChildIndex < Count && _heap[leftChildIndex].Priority.CompareTo(_heap[smallestChildIndex].Priority) < 0)
            {
                smallestChildIndex = leftChildIndex;
            }
            if (rightChildIndex < Count && _heap[rightChildIndex].Priority.CompareTo(_heap[smallestChildIndex].Priority) < 0)
            {
                smallestChildIndex = rightChildIndex;
            }

            // Если наименьший элемент не является текущим родителем, меняем их местами.
            if (smallestChildIndex != index)
            {
                Swap(index, smallestChildIndex);
                // Продолжаем просеивание вниз от новой позиции элемента.
                SiftDown(smallestChildIndex);
            }
        }

        /// <summary>
        /// Вспомогательный метод для обмена двух элементов в куче.
        /// </summary>
        private void Swap(int index1, int index2)
        {
            var temp = _heap[index1];
            _heap[index1] = _heap[index2];
            _heap[index2] = temp;
        }
    }
}