using System.Collections.Generic;

namespace btest.MyBase
{

    public class ConfigNodeImpl: ConfigNode
    {
    }

    public abstract class ConfigNode
    {
        public virtual void StartTran(ITranContext context)
        {
            //add transaction
            AddTransaction(context);

            //add messages
            AddMessages(context);
        }

        #region private
        void AddTransaction(ITranContext context)
        {

        }

        void AddMessages(ITranContext context)
        {
            var cars = context.GetTranCars();
            //add messages with cars
            foreach(var car in cars)
            {
                AddMessage(car);
            }
        }

        void AddMessage(ITransactionCar car)
        {

        }
        #endregion

    }

    public interface ITranContext
    {
        List<ITransactionCar> GetTranCars();
    }

    public interface ITransaction
    {

    }
    public interface ITransactionCar
    {

    }


}
