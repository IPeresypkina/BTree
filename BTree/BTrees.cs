namespace BTree
{
    public class BTrees
    {
        public Node root;
        public int degree; 
        

        public BTrees(int _degree)
        {
            root = null;
            degree = _degree;
        }

        // функция для обхода дерева 
        internal int[] traverse()
        {
            int[] masRoot = new int[root.n];
            if (root != null)
                root.traverse(masRoot);
            return masRoot;
        }

        // функция для поиска ключа в этом дереве 
        internal Node search(int k)
        {
            return (root == null) ? null : root.search(k);
        }

        // Основная функция, которая вставляет новый ключ в это B-Tree 
        internal void insert(int k)
        {
            if (root == null)
            {
                root = new Node(degree, true);
                root.keys[0] = k; // Вставить ключ 
                root.n = 1; // Обновить количество ключей в корне 
            }
            else  
            {
                // Если корень полон, то дерево растет в высоту
                if (root.n == 2 * degree - 1)
                {
                    // Выделите память для нового рута
                    Node s = new Node(degree, false);
                    // Сделать старый корень дочерним для нового корня
                    s.C[0] = root;
                    // Разделите старый корень и переместите 1 ключ к новому корню
                    s.splitChild(0, root);
                    // У нового корня теперь двое детей. Решите, у кого из двух детей будет новый ключ
                    int i = 0;
                    if (s.keys[0] < k)
                        i++;
                    s.C[i].insertNonFull(k);
                    // Изменить корень 
                    root = s;
                }
                else // Если root не заполнен,
                    root.insertNonFull(k);
            }
        }
        
        internal void remove(int k)
        {
            // Вызовите функцию удаления для root
            root.remove(k);

            // Если корневой узел имеет 0 ключей, сделайте его первым потомком новым корнем,
            // если у него есть потомок, в противном случае установите root как NULL.
            if (root.n == 0)
            {
                Node tmp = root;
                if (root.leaf)
                    root = null;
                else
                    root = root.C[0];
            }
            return;
        }
    }
}