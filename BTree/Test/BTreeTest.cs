using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using BTree;

namespace BTree.Test
{
    [TestFixture]
    public class BTreeTest
    {
        [Test]
        public void CanCreateBTree()//инициализация
        {
            BTrees tree = new BTrees(3);
            Assert.True(tree != null);//дерева не null
            Assert.AreEqual(3, tree.degree); //степень
            Assert.True(tree.root == null); //корень
        }
        
        [Test]
        public void CanCreateRoot()//создание корня
        {
            Node root = new Node(3, true);
            Assert.AreEqual(3, root.degree); //степень
            Assert.AreEqual(true, root.leaf); //узел лист
            Assert.AreEqual(5, root.keys.Length); // ключей от 1 до 2*degree-1 
            Assert.AreEqual(6, root.C.Length); // количества дочерних указателей 2*degree
            Assert.AreEqual(0, root.n); //количество ключей 0
        }

        [Test]
        public void CanInsertValuesIntoBTree()
        {
            BTrees tree = new BTrees(3);
            tree.insert(1);
            tree.insert(2);
            tree.insert(3);
            tree.insert(4);
            tree.insert(5);
            tree.insert(6);
            tree.insert(7);
            tree.insert(8);
            tree.insert(9);
            tree.insert(10);
            tree.insert(11);
            tree.insert(12);
            tree.insert(13);
            tree.insert(14);
            tree.insert(15);
            Assert.AreEqual(3, tree.degree);
            Assert.IsNotEmpty(tree.traverse());
        }
        
        [Test]
        public void CanFindTheValueInTheBTree()
        {
            BTrees tree = new BTrees(2);
            tree.insert(1);
            tree.insert(2);
            tree.insert(3);
            tree.insert(4);
            Assert.IsNotNull(tree.search(4));
            Assert.IsNotNull(tree.search(1));
            Assert.AreEqual(null, tree.search(5));
        }
        
        [Test]
        public void CanRemoveValueFromTreePredecessor()//удаление корневого элемента
        {
            BTrees tree = new BTrees(2);
            tree.insert(10);
            tree.insert(20);
            tree.insert(30);
            tree.insert(40);
            tree.insert(50);
            tree.insert(60);
            tree.insert(70);
            tree.insert(80);
            tree.insert(90);
            tree.insert(100);
            tree.insert(110);
            tree.insert(120);
            tree.insert(130);
            tree.insert(140);
            tree.insert(1);
            tree.insert(2);
            tree.insert(3);
            tree.insert(0);
            Assert.IsNotNull(tree.search(40));
            tree.remove(40);
            Assert.AreEqual(null, tree.search(40));
        }
        
        [Test]
        public void CanRemoveValueFromTreeSheet()//Если ключ k находится в узле x и x является листом, удалите ключ k из x
        {
            BTrees tree = new BTrees(3);
            tree.insert(1); 
            tree.insert(3); 
            tree.insert(7); 
            tree.insert(10); 
            tree.insert(11); 
            tree.insert(13); 
            tree.insert(14); 
            tree.insert(15); 
            tree.insert(18); 
            tree.insert(16); 
            tree.insert(19); 
            tree.insert(24); 
            tree.insert(25); 
            tree.insert(26); 
            tree.insert(21); 
            tree.insert(4); 
            tree.insert(5); 
            tree.insert(20); 
            tree.insert(22); 
            tree.insert(2); 
            tree.insert(17); 
            tree.insert(12); 
            tree.insert(6);
            Assert.IsNotNull(tree.search(4));
            tree.remove(4);
            Assert.AreEqual(null, tree.search(4));
        }
        
        [Test]
        public void CanRemoveValueFromTreeSuccessor()//Если ключ k находится в узле x, а x является внутренним узлом
        {
            BTrees tree = new BTrees(3);
            tree.insert(1); 
            tree.insert(3); 
            tree.insert(7); 
            tree.insert(10); 
            tree.insert(11); 
            tree.insert(13); 
            tree.insert(14); 
            tree.insert(15); 
            tree.insert(18); 
            tree.insert(16); 
            tree.insert(19); 
            tree.insert(24); 
            tree.insert(25); 
            tree.insert(26); 
            tree.insert(21); 
            tree.insert(4); 
            tree.insert(5); 
            tree.insert(20); 
            tree.insert(22); 
            tree.insert(2); 
            tree.insert(17); 
            tree.insert(12); 
            tree.insert(6);
            Assert.IsNotNull(tree.search(13));
            Assert.IsNotNull(tree.search(7));
            tree.remove(13);
            Assert.AreEqual(null, tree.search(13));
            tree.remove(7);
            Assert.AreEqual(null, tree.search(7));
        }
        
        [Test]
        public void CanRemoveValueFromSubtree()
        {
            BTrees tree = new BTrees(3);
            tree.insert(1); 
            tree.insert(3); 
            tree.insert(7); 
            tree.insert(10); 
            tree.insert(11); 
            tree.insert(13); 
            tree.insert(14); 
            tree.insert(15); 
            tree.insert(18); 
            tree.insert(16); 
            tree.insert(19); 
            tree.insert(24); 
            tree.insert(25); 
            tree.insert(26); 
            tree.insert(21); 
            tree.insert(4); 
            tree.insert(5); 
            tree.insert(20); 
            tree.insert(22); 
            tree.insert(2); 
            tree.insert(17); 
            tree.insert(12); 
            tree.insert(6);
            
            Assert.IsNotNull(tree.search(6));
            tree.remove(6);
            
            Assert.IsNotNull(tree.search(13));
            Assert.IsNotNull(tree.search(7));
            tree.remove(13);
            Assert.AreEqual(null, tree.search(13));
            tree.remove(7);
            Assert.AreEqual(null, tree.search(7));
            
            Assert.IsNotNull(tree.search(4));
            Assert.IsNotNull(tree.search(2));
            tree.remove(4);
            Assert.AreEqual(null, tree.search(4));
            tree.remove(2);
            Assert.AreEqual(null, tree.search(2));
            
            Assert.IsNotNull(tree.search(16));
            tree.remove(16);
            
        }

    }
}