namespace BTree
{
    public class Node
    {
        public int[] keys;  // Массив ключей
        public int degree;  // Минимальная степень (определяет диапазон для количества ключей)
        public Node[] C; // Массив дочерних указателей 
        public int n;     // Текущее количество ключей 
        public bool leaf; // Истинно, когда узел является листом. В противном случае ложь

        public Node(int _degree, bool _leaf)
        {
            degree = _degree;
            leaf = _leaf;
            
            keys = new int[2 * degree - 1];
            C = new Node[2 * degree];
            n = 0;
        }

        // Функция для прохождения всех узлов в поддереве с корнем из этого узла
        internal void traverse(int[] masRoot)
        {
            // Существует n ключей и n + 1 дочерних элементов, обходы через n ключей и первых n дочерних элементов
            int i;
            for (i = 0; i < n; i++)
            {
                if (leaf == false)
                    C[i].traverse(masRoot);
                masRoot[i] = keys[i];
                //Console.Write(" " + keys[i]);
            }
            
            if (leaf == false)
                C[i].traverse(masRoot);
        }

        // Функция для поиска ключа в поддереве с корнем этого узла.     
        internal Node search(int k)
        {
            int i = 0;
            while (i < n && k > keys[i])
                i++;
            
            if (keys[i] == k)
                return this;
            
            if (leaf == true)
                return null;
            
            return C[i].search(k);
        }

        // Вспомогательная функция для вставки нового ключа в поддерево с корнем этого узла.
        // Предполагается, что узел должен быть не заполнен при вызове этой функции.
        internal void insertNonFull(int k)
        {
            // Инициализировать индекс как индекс самого правого элемента 
            int i = n - 1;
            
            if (leaf == true)
            {
                // Следующий цикл выполняет две вещи:
                // а) Находит местоположение нового ключа для вставки
                // б) Перемещает все большие ключи на одно место впереди
                while (i >= 0 && keys[i] > k)
                {
                    keys[i + 1] = keys[i];
                    i--;
                }

                // Вставьте новый ключ в найденное место
                keys[i + 1] = k;
                n = n + 1;
            }
            else 
            {
                // Найдите ребенка, у которого будет новый ключ
                while (i >= 0 && keys[i] > k)
                    i--;
                
                if (C[i + 1].n == 2 * degree - 1)
                {
                    splitChild(i + 1, C[i + 1]);

                    // После разделения средний ключ C[i] идет вверх,
                    // а C[i] разделяется на два.
                    // Посмотрите, какой из двух будет иметь новый ключ
                    if (keys[i + 1] < k)
                        i++;
                }
                C[i + 1].insertNonFull(k);
            }
        }

        // Вспомогательная функция для разделения дочернего элемента y 
        internal void splitChild(int i, Node y)
        {
            // Создайте новый узел, который будет хранить (t-1) ключей y
            Node z = new Node(y.degree, y.leaf);
            z.n = degree - 1;

            // Скопируйте последние(t-1) ключи y в z
            for (int j = 0; j < degree - 1; j++)
                z.keys[j] = y.keys[j + degree];

            // Скопируйте последние t детей y в z
            if (y.leaf == false)
            {
                for (int j = 0; j < degree; j++)
                    z.C[j] = y.C[j + degree];
            }

            // Уменьшить количество ключей в y
            y.n = degree - 1;

            // Поскольку у этого узла будет новый дочерний элемент, создайте пространство нового дочернего элемента.
            for (int j = n; j >= i + 1; j--)
                C[j + 1] = C[j];

            // Свяжите нового ребенка с этим узлом 
            C[i + 1] = z;

            // Ключ y переместится в этот узел. Найдите местоположение нового ключа и переместите все большие ключи на один пробел вперед.
            for (int j = n - 1; j >= i; j--)
                keys[j + 1] = keys[j];

            // Скопируйте средний ключ у в этот узел
            keys[i] = y.keys[degree - 1];

            // Увеличение количества ключей в этом узле
            n = n + 1;
        }
        
        //ФУНКЦИИ ДЛЯ УДАЛЕНИЯ

        // Вспомогательная функция, которая возвращает индекс первого ключа, который больше или равен k
        int findKey(int k)
        {
            int idx = 0;
            while (idx < n && keys[idx] < k)
                ++idx;
            return idx;
        }

        // Функция для удаления ключа k из поддерева, укорененного в этом узле
        internal void remove(int k)
        {
            int idx = findKey(k);

            // Ключ для удаления присутствует в этом узле
            if (idx < n && keys[idx] == k)
            {

                // Если узел является листовым узлом - вызывается removeFromLeaf
                // В противном случае вызывается функция removeFromNonLeaf.
                if (leaf)
                    removeFromLeaf(idx);
                else
                    removeFromNonLeaf(idx);
            }
            else
            {
                // Ключ, который необходимо удалить, присутствует в поддереве с корнем этого узла.
                // Флаг указывает, присутствует ли ключ в поддереве с корнем последнего потомка этого узла.
                bool flag = ((idx == n) ? true : false);

                // Если дочерний элемент, в котором предполагается наличие ключа, имеет менее t ключей, мы заполняем этот дочерний элемент
                if (C[idx].n < degree)
                    fill(idx);

                // Если последний дочерний элемент был объединен, он должен быть объединен с предыдущим дочерним элементом,
                // и поэтому мы возвращаемся к (idx-1) -ому дочернему элементу.
                // Иначе, мы возвращаемся к (idx) -ому дочернему элементу, который теперь имеет по крайней мере t ключей.
                if (flag && idx > n)
                    C[idx - 1].remove(k);
                else
                    C[idx].remove(k);
            }
            return;
        }

        // Функция для удаления ключа idx из этого узла, который является листовым узлом
        void removeFromLeaf(int idx)
        {
            // Переместите все ключи после idx-го на одно место назад
            for (int i = idx + 1; i < n; ++i)
            {
                keys[i - 1] = keys[i];
            }
            // Уменьшить количество ключей
            n--;
        }

        //Функция для удаления ключа idx-го из этого узла, который является неконечным узлом
        void removeFromNonLeaf(int idx)
        {

            int k = keys[idx];

            // Если дочерний элемент, предшествующий k (C [idx]), имеет по крайней мере t ключей,
            // найдите предшественника 'pred' для k в поддереве с корнем в C [idx].
            // Заменить k на пред. Рекурсивно удалить pred в C [idx].
            if (C[idx].n >= degree)
            {
                int pred = getPred(idx);
                keys[idx] = pred;
                C[idx].remove(pred);
            }

            // Если у дочернего элемента C [idx] меньше t ключей, проверьте C [idx + 1].
            // Если C[idx + 1] имеет по крайней мере t ключей, найдите преемника 'succ' из k в поддереве с корнем в C[idx + 1]
            // Заменить k на succ
            // Рекурсивно удалить succ в C[idx + 1]
            else if (C[idx + 1].n >= degree)
            {
                int succ = getSucc(idx);
                keys[idx] = succ;
                C[idx + 1].remove(succ);
            }

            // Если и C [idx], и C [idx + 1] имеют менее t ключей, объедините k и все C [idx + 1] в C [idx]
            // Теперь C[idx] содержит 2t - 1 ключей
            // Освободите C[idx + 1] и рекурсивно удалите k из C[idx]
            else
            {
                merge(idx);
                C[idx].remove(k);
            }
            return;
        }


        // Функция для получения предшественника ключей [idx]
        int getPred(int idx)
        {
            // Двигайтесь к самому правому узлу, пока не достигнете листа
            Node cur = C[idx];
            while (!cur.leaf)
                cur = cur.C[cur.n];

            // Верните последний ключ листа
            return cur.keys[cur.n - 1];
        }


        int getSucc(int idx)
        {

            // Продолжайте перемещать самый левый узел, начиная с C [idx + 1], пока мы не достигнем листа
            Node cur = C[idx + 1];
            while (!cur.leaf)
                cur = cur.C[0];

            // Верните первый ключ листа
            return cur.keys[0];
        }

        // Функция для заполнения дочернего C [idx], который имеет менее чем t-1 ключей
        void fill(int idx)
        {
            // Если предыдущий дочерний элемент (C [idx-1]) имеет более t-1 ключей, позаимствуйте ключ у этого дочернего элемента.
            if (idx != 0 && C[idx - 1].n >= degree)
                borrowFromPrev(idx);

            // Если следующий дочерний элемент (C [idx + 1]) имеет более t-1 ключей, позаимствуйте ключ у этого дочернего элемента.
            else if (idx != n && C[idx + 1].n >= degree)
                borrowFromNext(idx);

            // Слияние C [idx] со своим родным братом
            // Если C[idx] является последним потомком, объедините его с предыдущим братом
            // В противном случае объединить его со своим следующим родным братом
            else
            {
                if (idx != n)
                    merge(idx);
                else
                    merge(idx - 1);
            }
            return;
        }

        // Функция для заимствования ключа из C [idx-1] и вставки его в C [idx]
        void borrowFromPrev(int idx)
        {

            Node child = C[idx];
            Node sibling = C[idx - 1];

            // Последний ключ из C [idx-1] переходит к родителю, а ключ [idx-1] из parent вставляется как первый ключ в C [idx].
            // Таким образом, брат теряет один ключ, а ребенок получает один ключ.

            // Перемещение всех клавиш в C [idx] на один шаг вперед
            for (int i = child.n - 1; i >= 0; --i)
                child.keys[i + 1] = child.keys[i];

            // Если C [idx] не является листом, переместите все его дочерние указатели на один шаг вперед
            if (!child.leaf)
            {
                for (int i = child.n; i >= 0; --i)
                    child.C[i + 1] = child.C[i];
            }

            // Установка первого ключа ребенка равным ключам [idx-1] из текущего узла
            child.keys[0] = keys[idx - 1];

            // Перемещение последнего ребенка братьев и сестер как первого ребенка C [idx]
            if (!child.leaf)
                child.C[0] = sibling.C[sibling.n];

            // Перемещение ключа от родного брата к родителю
            // Это уменьшает количество ключей в брате
            keys[idx - 1] = sibling.keys[sibling.n - 1];

            child.n += 1;
            sibling.n -= 1;

            return;
        }


        // Функция для заимствования ключа из C [idx + 1] и помещения его в C [idx]
        void borrowFromNext(int idx)
        {

            Node child = C[idx];
            Node sibling = C[idx + 1];

            // Ключи [IDX] вставляется как последний ключ в C [IDX]
            child.keys[(child.n)] = keys[idx];

            // Первый дочерний элемент брата вставляется как последний дочерний элемент в C [idx]
            if (!(child.leaf))
                child.C[(child.n) + 1] = sibling.C[0];

            //Первый ключ от брата вставлен в ключи [idx]
            keys[idx] = sibling.keys[0];

            // Перемещение всех ключей в брате на один шаг позади
            for (int i = 1; i < sibling.n; ++i)
                sibling.keys[i - 1] = sibling.keys[i];

            // Перемещение указателей ребенка на один шаг позади
            if (!sibling.leaf)
            {
                for (int i = 1; i <= sibling.n; ++i)
                    sibling.C[i - 1] = sibling.C[i];
            }

            // Увеличение и уменьшение количества клавиш C [idx] и C [idx + 1] соответственно
            child.n += 1;
            sibling.n -= 1;

            return;
        }


        // Функция для объединения C [idx] с C [idx + 1]
        // C[idx + 1] освобождается после слияния
        void merge(int idx)
        {
            Node child = C[idx];
            Node sibling = C[idx + 1];

            // Вытащив ключ из текущего узла и вставив его в (t-1) -ю позицию C [idx]
            child.keys[degree - 1] = keys[idx];

            // Копирование ключей из C [idx + 1] в C [idx] в конце
            for (int i = 0; i < sibling.n; ++i)
                child.keys[i + degree] = sibling.keys[i];

            // Копирование дочерних указателей из C [idx + 1] в C [idx]
            if (!child.leaf)
            {
                for (int i = 0; i <= sibling.n; ++i)
                    child.C[i + degree] = sibling.C[i];
            }

            // Перемещение всех ключей после idx в текущем узле на один шаг раньше -
            // чтобы заполнить пробел, созданный перемещением клавиш [idx] в C [idx]
            for (int i = idx + 1; i < n; ++i)
                keys[i - 1] = keys[i];

            // Перемещение дочерних указателей после (idx + 1) в текущем узле на один шаг раньше
            for (int i = idx + 2; i <= n; ++i)
                C[i - 1] = C[i];

            // Обновление подсчета ключей дочернего и текущего узла
            child.n += sibling.n + 1;
            n--;
            return;
        }
    }
}