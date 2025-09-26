using System;
using UnityEngine;

namespace __SCRIPTS
{
	public class AttackBuilder
	{
		private Life _originLife;
		private Life _destinationLife;
		private Vector2 _originFloorPoint;
		private Vector2 _destinationFloorPoint;
		private float _originHeight = 5f;
		private float _destinationHeight = 5f;
		private float _damageAmount;
		private float _extraPush;
		private bool _isPoison;
		private Color _color = Color.red;
		private bool _causesFlying;

		private AttackBuilder()
		{
		}

		public static AttackBuilder Create() => new();

		public AttackBuilder FromLife(Life originLife)
		{
			_originLife = originLife;
			_originFloorPoint = originLife.transform.position;
			_originHeight = originLife.AttackHeight;
			return this;
		}

		public AttackBuilder ToLife(Life destinationLife)
		{
			_destinationLife = destinationLife;
			_destinationFloorPoint = destinationLife.transform.position;
			_destinationHeight = destinationLife.AttackHeight;
			return this;
		}

		public AttackBuilder FromPoint(Vector2 originFloorPoint)
		{
			_originFloorPoint = originFloorPoint;
			_originHeight = 0;
			return this;
		}

		public AttackBuilder ToPoint(Vector2 destinationFloorPoint)
		{
			_destinationFloorPoint = destinationFloorPoint;
			_destinationHeight = 0;
			return this;
		}

		public AttackBuilder WithDamage(float damageAmount)
		{
			_damageAmount = damageAmount;
			return this;
		}

		public AttackBuilder WithExtraPush(float extraPush)
		{
			_extraPush = extraPush;
			return this;
		}

		public AttackBuilder AsPoison(bool isPoison = true)
		{
			_isPoison = isPoison;
			return this;
		}

		public AttackBuilder WithColor(Color color)
		{
			_color = color;
			return this;
		}

		public AttackBuilder WithFlying(bool flying)
		{
			_causesFlying = flying;
			return this;
		}

		public Attack Build()
		{
			Attack attack;

			if (_originLife != null && _destinationLife != null)
				attack = new Attack(_originLife, _destinationLife, _damageAmount);
			else if (_originLife != null)
				attack = new Attack(_originLife, _originFloorPoint, _destinationFloorPoint, _damageAmount, _extraPush);
			else
				throw new InvalidOperationException("Must specify OriginLife");

			if (_originFloorPoint != Vector2.zero)
				attack.OriginFloorPoint = _originFloorPoint;
			if (_destinationFloorPoint != Vector2.zero)
				attack.DestinationFloorPoint = _destinationFloorPoint;

			attack.OriginHeight = _originHeight;
			attack.DestinationHeight = _destinationHeight;
			attack.ExtraPush = _extraPush;
			attack.IsPoison = _isPoison;
			attack.color = _color;
			attack.CausesFlying = _causesFlying;

			return attack;
		}
	}
}
