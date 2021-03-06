﻿#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SF.Sys.Tests.NumberValueTypes;
using System.ComponentModel.DataAnnotations;
using SF.Sys.TimeServices;
using SF.Sys.Linq;
using SF.Sys.Reflection;

namespace SF.Sys.Tests.SampleGenerators
{

	class DefaultSampleGeneratorProvider : IValueSampleGeneratorProvider
	{
		INumberValueTypeProvider NumberValueTypeProvider { get; }
		ITimeService TimeService { get; }
		public DefaultSampleGeneratorProvider(ITimeService TimeService ,INumberValueTypeProvider NumberValueTypeProvider)
		{
			this.TimeService = TimeService;
			this.NumberValueTypeProvider = NumberValueTypeProvider;
		}



		public class NumberSampleGenerator<T> : IValueSampleGenerator<T>
		{
			INumberValueType<T> ValueType { get; }
			T MaxValue { get; }
			T MinValue { get; } 
			public NumberSampleGenerator(INumberValueTypeProvider NumberValueTypeProvider,PropertyInfo Prop)
			{
				this.ValueType = NumberValueTypeProvider.GetNumberValueType<T>();
				var range = Prop?.GetCustomAttribute<RangeAttribute>();
				MaxValue = range?.Maximum== null ? ValueType.MinValue : (T)Convert.ChangeType(range.Minimum, typeof(T));
				MinValue = range?.Minimum == null ? ValueType.MaxValue: (T)Convert.ChangeType(range.Maximum, typeof(T));
			}
			public bool NextSampleSupported => true;

			public IEnumerable<T> ValidSamples
			{
				get
				{
					var two = ValueType.Add(ValueType.One, ValueType.One);
					var mid = ValueType.Add(
						ValueType.Divide(MinValue, two),
						ValueType.Divide(MaxValue, two)
						);
					var re = new List<T>()
					{
						MinValue,
						MaxValue,
					};
					if (ValueType.Compare(mid, MinValue) > 0 && ValueType.Compare(mid, MaxValue) < 0)
						re.Add(mid);
					return re;
				}
			}

			public IEnumerable<T> InvalidSamples
			{
				get
				{
					var re = new List<T>();
					if (ValueType.Compare(MinValue, ValueType.MinValue) > 0)
						re.Add(ValueType.Subtract(MinValue, ValueType.One));
					if (ValueType.Compare(MaxValue, ValueType.MaxValue) < 0)
						re.Add(ValueType.Add(MaxValue, ValueType.One));
					return re;
				}
			}

			public T NextSample(T OrgValue,ISampleSeed Seed)
			{
				return ValueType.Add(OrgValue, ValueType.ConvertFrom(Seed.NextValue()));
			}

		}

		class StringSampleGenerator : IValueSampleGenerator<string>
		{
			public int MaxLength { get; }
			public int MinLength { get; }

			public bool NextSampleSupported => true;

			public IEnumerable<string> ValidSamples
			{
				get
				{
					var re = new List<string>();
					if (MinLength == 0)
					{
						re.Add(null);
						re.Add("");
					}
					re.Add(Strings.LowerChars.Random(MinLength));
					re.Add(Strings.LowerChars.Random((MinLength + MaxLength) / 2));
					if (MaxLength <= 1024 * 1024)
						re.Add(Strings.LowerChars.Random(MaxLength));
					return re;

				}
			}

			public IEnumerable<string> InvalidSamples {
				get
				{
					var re = new List<string>();
					if (MinLength > 0)
					{
						re.Add(null);
						re.Add("");
					}
					if (MinLength > 1)
						re.Add(Strings.LowerChars.Random(MinLength - 1));

					if (MaxLength <= 1024 * 1024)
						re.Add(
							Strings.LowerChars.Random(MaxLength + 1)
							);
					return re;

				}
			}

			public string NextSample(string OrgValue, ISampleSeed Seed)
			{
				return Strings.LowerChars.Random(4);
			}
			public StringSampleGenerator(PropertyInfo Prop)
			{
				var strlen = Prop?.GetCustomAttribute<StringLengthAttribute>();
				var maxlen = Prop?.GetCustomAttribute<MaxLengthAttribute>();
				var minlen = Prop?.GetCustomAttribute<MinLengthAttribute>();
				var required= Prop?.GetCustomAttribute<RequiredAttribute>();

				MaxLength = Math.Min(
					strlen?.MinimumLength ?? int.MaxValue,
					maxlen?.Length ?? int.MaxValue
					);

				MinLength = Math.Max(
					Math.Max(
						strlen?.MaximumLength ?? 0,
						minlen?.Length ?? 0
					),
					required == null ? 0 : 1
					);
			}
		}
		class EnumSampleGenerator<T> : IValueSampleGenerator<T>
		{
			static T[] Values { get; } = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
			public EnumSampleGenerator(PropertyInfo Prop)
			{
			}

			public bool NextSampleSupported => true;

			public IEnumerable<T> ValidSamples { get; }= Values;
			static E[] GetEnumValues<E>() =>
				(E[])Enum.GetValues(typeof(E));
			
			public IEnumerable<T> InvalidSamples { get; } = new T[]
			{
				(T)Enum.ToObject(typeof(T),GetEnumValues<T>().Select(v=> (int)Convert.ChangeType(v,typeof(int))).Min()-1),
				(T)Enum.ToObject(typeof(T),GetEnumValues<T>().Select(v=> (int)Convert.ChangeType(v,typeof(int))).Max()+1)
			};

			public T NextSample(T OrgValue, ISampleSeed Seed)
			{
				return Values[Seed.NextValue() % Values.Length];
			}
		}
		class BoolSampleGenerator : IValueSampleGenerator<bool>
		{
			public BoolSampleGenerator(PropertyInfo Prop)
			{
			}

			public bool NextSampleSupported => true;

			public IEnumerable<bool> ValidSamples { get; } = new[] { false, true };

			public IEnumerable<bool> InvalidSamples => Enumerable.Empty<bool>();

			public bool NextSample(bool OrgValue, ISampleSeed Seed)
			{
				return (((OrgValue?1:0) + Seed.NextValue()) % 2) == 1;
			}
		}

		class DateTimeSampleGenerator : IValueSampleGenerator<DateTime>
		{
			ITimeService TimeService { get; }
			public DateTimeSampleGenerator(ITimeService TimeService,PropertyInfo Prop)
			{
				this.TimeService = TimeService;
			}

			public bool NextSampleSupported => true;

			public IEnumerable<DateTime> ValidSamples => new[]
			{
				TimeService.Now,
				TimeService.Now.AddSeconds(1),
				TimeService.Now.AddHours(1),
				TimeService.Now.AddDays(1)
			};

			public IEnumerable<DateTime> InvalidSamples => Enumerable.Empty<DateTime>();

			public DateTime NextSample(DateTime OrgValue, ISampleSeed Seed)
			{
				return OrgValue.AddSeconds(Seed.NextValue());
			}
		}
		class TimeSpanSampleGenerator : IValueSampleGenerator<TimeSpan>
		{
			public TimeSpanSampleGenerator(PropertyInfo Prop)
			{
			}

			public bool NextSampleSupported => true;

			public IEnumerable<TimeSpan> ValidSamples { get; }= new[]
			{
				TimeSpan.FromSeconds(0),
				TimeSpan.FromSeconds(1),
				TimeSpan.FromMinutes(1),
				TimeSpan.FromHours(1),
				TimeSpan.FromDays(1)
			};

			public IEnumerable<TimeSpan> InvalidSamples => Enumerable.Empty<TimeSpan>();

			public TimeSpan NextSample(TimeSpan OrgValue, ISampleSeed Seed)
			{
				return OrgValue.Add(TimeSpan.FromSeconds(Seed.NextValue()));
			}
		}
		class NullableSampleGenerator<T> : IValueSampleGenerator<Nullable<T>>
			where T:struct
		{
			IValueSampleGenerator<T> BaseGenerator { get; }

			public bool NextSampleSupported => BaseGenerator.NextSampleSupported;

			public IEnumerable<Nullable<T>> ValidSamples =>
				BaseGenerator.ValidSamples.Select(i=>new Nullable<T>(i)).WithFirst(null);

			public IEnumerable<Nullable<T>> InvalidSamples =>
				BaseGenerator.InvalidSamples.Select(i => new Nullable<T>(i));

			public NullableSampleGenerator(IValueSampleGeneratorProvider Provider ,PropertyInfo Prop)
			{
				this.BaseGenerator = Provider.GetGenerator<T>(Prop);
			}

			public Nullable<T> NextSample(Nullable<T> OrgValue, ISampleSeed Seed)
			{
				return BaseGenerator.NextSample(OrgValue ?? default(T), Seed);
			}
		}

		
		public IValueSampleGenerator<T> GetGenerator<T>(PropertyInfo Prop)
		{
			var type = typeof(T);
			if (Prop != null && !(
				Prop.PropertyType == typeof(T) ||
				(Prop.PropertyType.IsGeneric() && Prop.PropertyType.GenericTypeArguments[0] == typeof(T)))
				)
				throw new ArgumentException($"属性{Prop}类型{Prop.PropertyType}和指定类型{typeof(T)}不符");

			if (type.IsGeneric() && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				return (IValueSampleGenerator<T>)Activator.CreateInstance(
					typeof(NullableSampleGenerator<>).MakeGenericType(type.GenericTypeArguments[0]),
					(IValueSampleGeneratorProvider)this,
					Prop
					);

			if (type.IsEnumType())
				return new EnumSampleGenerator<T>(Prop);

			if (type == typeof(bool))
				return (IValueSampleGenerator<T>)new BoolSampleGenerator(Prop);


			if (type.IsNumberLikeType())
				return new NumberSampleGenerator<T>(NumberValueTypeProvider, Prop);

			if (type == typeof(string))
				return (IValueSampleGenerator<T>)new StringSampleGenerator(Prop);
			if (type == typeof(DateTime))
				return (IValueSampleGenerator<T>)new DateTimeSampleGenerator(TimeService,Prop);

			if (type == typeof(TimeSpan))
				return (IValueSampleGenerator<T>)new TimeSpanSampleGenerator(Prop);

			return null;
		}

	}
}
