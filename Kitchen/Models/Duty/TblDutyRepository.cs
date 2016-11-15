using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcStudy.Models
{
    public partial class KitchenRepository : IKitchenRepository
    {
        public List<tbl_duty> CalculateDutyPeople(bool forFutureWeek, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                var searchDate = forFutureWeek ? DateTime.Now.AddDays(7).Date : DateTime.Now.Date;
                var startDateOfWeek = GetStartDateOfWeek(searchDate);
                var endDateOfWeek = startDateOfWeek.AddDays(6);
                var menuDaysCount =
                    DbCtx.tbl_menu.Where(x => x.date >= startDateOfWeek && x.date <= endDateOfWeek)
                        .Select(x => x.date)
                        .Distinct().Count();
                var result = new List<tbl_duty>(menuDaysCount);
                var futureWeekPeople = DbCtx.tbl_menu_selection.Where(x => x.tbl_menu.date <= endDateOfWeek
                                                                           && x.tbl_menu.date >= startDateOfWeek &&
                                                                           !(x.tbl_people.excepts_duty ?? false))
                    .Select(x => x.id_people).Distinct();

                var startDateOfLastDuty = startDateOfWeek.AddDays(-futureWeekPeople.Count());
                var endDateOfLastDuty = startDateOfWeek.AddDays(-1);
                var futurePeopleDutyDct = new Dictionary<long, int>();

                foreach (var peopleId in futureWeekPeople)
                {
                    var dutyCnt =
                        DbCtx.tbl_duty.Count(
                            x =>
                                x.id_people == peopleId && x.duty_date >= startDateOfLastDuty &&
                                x.duty_date <= endDateOfLastDuty);

                    if (!futurePeopleDutyDct.ContainsKey(peopleId))
                        futurePeopleDutyDct.Add(peopleId, dutyCnt);
                }

                var minDutyCount = futurePeopleDutyDct.Values.Min();
                var maxDutyCount = futurePeopleDutyDct.Values.Max();
                var dctDate = startDateOfWeek;

                while (result.Count < menuDaysCount)
                {
                    if (!futurePeopleDutyDct.Any())
                        break;

                    var peopleWithMinLastDuty =
                        futurePeopleDutyDct.Where(
                            x => x.Value == minDutyCount && result.All(y => y.id_people != x.Key));

                    foreach (var people in peopleWithMinLastDuty)
                        if (result.All(x => x.duty_date != dctDate))
                        {
                            if (result.Count == menuDaysCount)
                                break;

                            result.Add(new tbl_duty
                            {
                                duty_date = dctDate,
                                id_people = people.Key,
                                tbl_people = GetUsers(new tbl_people {id = people.Key}, out error).FirstOrDefault()
                            });
                            dctDate = dctDate.AddDays(1);
                        }

                    if (maxDutyCount == minDutyCount)
                        break;

                    minDutyCount++;
                }

                if (!result.Any())
                    throw new Exception("Дежурных не нашлось");

                if (result.Count < menuDaysCount)
                {
                    var nextDate = result.Max(x => x.duty_date);

                    while (result.Count < menuDaysCount)
                    {
                        var tmpDct = new List<tbl_duty>();

                        foreach (var dutyItm in result)
                        {
                            nextDate = nextDate.AddDays(1);
                            tmpDct.Add(new tbl_duty
                            {
                                duty_date = nextDate,
                                id_people = dutyItm.id_people,
                                tbl_people = dutyItm.tbl_people
                            });
                        }

                        foreach (var tmpItm in tmpDct)
                        {
                            if (result.Count == menuDaysCount)
                                break;

                            result.Add(tmpItm);
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

        public tbl_duty[] GetDutyRecords(tbl_duty filter, out string error)
        {
            error = string.Empty;
            tbl_duty[] result;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (filter != null)
                    result =
                        DbCtx.tbl_duty.Where(
                            x => x.id_people == filter.id_people && x.duty_date.Date == filter.duty_date.Date)
                            .ToArray();
                else
                    result = DbCtx.tbl_duty.ToArray();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }

            return result;
        }

        public bool AcceptDutyPeople(List<tbl_duty> dutyPeopleDct, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                foreach (var dutyItm in dutyPeopleDct)
                {
                    var dutyRecByDate =
                        GetDutyRecords(null, out error)
                            .FirstOrDefault(x => x.duty_date.Date == dutyItm.duty_date.Date);

                    if (dutyRecByDate != null)
                    {
                        if (dutyRecByDate.id_people != dutyItm.id_people)
                        {
                            dutyRecByDate.id_people = dutyItm.id_people;

                            if (DbCtx.SaveChanges() <= 0)
                                throw new Exception("Не удалось добавить дежурного!");
                        }

                        continue;
                    }

                    var newDutyRec = new tbl_duty
                    {
                        id = GetTblDutyNextId(),
                        duty_date = dutyItm.duty_date,
                        id_people = dutyItm.id_people
                    };
                    DbCtx.tbl_duty.Add(newDutyRec);

                    if (DbCtx.SaveChanges() <= 0)
                        throw new Exception("Не удалось добавить дежурного!");
                }

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public bool UpdateDutyMan(tbl_duty dutyRecord, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (dutyRecord == null || dutyRecord.id < 1 || dutyRecord.id_people < 1)
                    throw new Exception("Не указаны основные параметры записи дежурства");

                var user = GetUsers(null, out error).FirstOrDefault(x => x.id == dutyRecord.id_people);

                if (user.excepts_duty ?? false)
                    throw new Exception("Пользователь не дежурит");

                var dutyRec = GetDutyRecords(null, out error).FirstOrDefault(x => x.id == dutyRecord.id);

                if (dutyRec == null)
                    throw new Exception("Не верно указаны основные параметры записи дежурства");

                dutyRec.id_people = dutyRecord.id_people;
                dutyRec.duty_date = dutyRecord.duty_date;

                if (DbCtx.SaveChanges() <= 0)
                    throw new Exception("Не удалось изменить запись о дежурстве");

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public long GetTblDutyNextId()
        {
            using (var dbContext = new kitchenEntities())
            {
                return dbContext.tbl_duty.Any() ? dbContext.tbl_duty.Max(x => x.id) + 1 : 1;
            }
        }

        public bool CheckForFutureWeekEaters(bool forFutureWeek, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                var searchDate = forFutureWeek ? DateTime.Now.AddDays(7).Date : DateTime.Now.Date;
                var startDateOfWeek = GetStartDateOfWeek(searchDate);
                var endDateOfWeek = startDateOfWeek.AddDays(6);
                return DbCtx.tbl_menu_selection.Any(x =>
                    !(x.tbl_people.excepts_duty ?? false) && x.tbl_menu.date >= startDateOfWeek &&
                    x.tbl_menu.date <= endDateOfWeek);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public bool PunishDutyMan(long peopleId, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                var punishDutyMan = DbCtx.tbl_people.FirstOrDefault(x => x.id == peopleId);

                if (punishDutyMan == null)
                    throw new Exception(string.Concat("Пользователь с ид ", peopleId, " не существует. ", error));

                var punishDate = DateTime.Now.AddDays(1).Date;
                var startDateOfWeek = GetStartDateOfWeek(DateTime.Now).Date;
                var endDateOfWeek = startDateOfWeek.AddDays(6);
                var changableDutyRecords =
                    DbCtx.tbl_duty.Where(x => x.duty_date >= punishDate && x.duty_date <= endDateOfWeek);

                if (changableDutyRecords == null || !changableDutyRecords.Any())
                    throw new Exception(string.Concat("Назначьте пользователя ", punishDutyMan.people_name,
                        " на следующyю неделю вручную"));

                endDateOfWeek = changableDutyRecords.Max(x => x.duty_date);
                var lastItm = changableDutyRecords.FirstOrDefault(dutyRec => dutyRec.duty_date == endDateOfWeek);
                DbCtx.tbl_duty.Remove(lastItm);

                foreach (var dutyItm in changableDutyRecords.Where(x => x.id != lastItm.id))
                    dutyItm.duty_date = dutyItm.duty_date.AddDays(1);

                DbCtx.tbl_duty.Add(new tbl_duty
                {
                    id = GetTblDutyNextId(),
                    duty_date = punishDate,
                    id_people = peopleId
                });

                if (DbCtx.SaveChanges() <= 0)
                    throw new Exception("Не удалось наказать дежурного");

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public bool ShowDutyAlert(long peopleId, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                var currentDate = DateTime.Now.Date;
                var dutyRec =
                    DbCtx.tbl_duty.FirstOrDefault(x => x.id_people == peopleId && x.duty_date == currentDate);
                return dutyRec != null;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

    }
}