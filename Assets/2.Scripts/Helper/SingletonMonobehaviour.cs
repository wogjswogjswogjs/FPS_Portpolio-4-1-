using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonobehaviour<T> : MonoBehaviour where T : Component
{
   private static T instance;

   public static T Instance
   {
      get => instance;
   }

   public static T GetOrCreateInstance()
   {
      if (instance == null)
      {
         instance = (T)FindObjectOfType(typeof(T));
         if (instance == null)
         {
            GameObject newGameObject = new GameObject(typeof(T).Name, typeof(T));
            instance = newGameObject.GetComponent<T>();
         }
      }

      return instance;
   }
   
   /// <summary>
   /// 이 클래스를 상속받은 클래스는 Awake함수를 따로 사용할려면
   /// 이 함수를 override한 후, base.Awake()를 호출한 후 사용해야 한다.
   /// </summary>
   protected virtual void Awake()
   {
      instance = this as T;
      if (Application.isPlaying == true)
      {
         if (transform.position != null && transform.root != null)
         {
            DontDestroyOnLoad(transform.root.gameObject);
         }
         else
         {
            DontDestroyOnLoad(gameObject);
         }
      }
   }
}
