using _PLUGINS.NaughtyAttributes.Scripts.Core.Utility;

namespace _PLUGINS.NaughtyAttributes.Scripts.Core.MetaAttributes
{
	public abstract class EnableIfAttributeBase : MetaAttribute
	{
		public string[] Conditions { get; private set; }
		public EConditionOperator ConditionOperator { get; private set; }
		public bool Inverted { get; protected set; }

		public EnableIfAttributeBase(string condition)
		{
			ConditionOperator = EConditionOperator.And;
			Conditions = new string[1] { condition };
		}

		public EnableIfAttributeBase(EConditionOperator conditionOperator, params string[] conditions)
		{
			ConditionOperator = conditionOperator;
			Conditions = conditions;
		}
	}
}
