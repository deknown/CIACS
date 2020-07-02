from django.shortcuts import render
from django.http import HttpResponse
from .models import Structure

# Create your views here.
def index(request):
    html = '<div style="margin-left: 20px; font-family: Geneva, sans-serif"><h1>Генерация отчёта</h1><p style="margin-bottom: 20px">При возникновении трудностей, обратитесь в техническую поддержку</p><hr/><h2>Фильтры</h2><p>Пожалуйста, выберите интересующую вас информацию:</p>'
    all_names = Structure.objects.all()
    inst = '<div style="display: flex; flex-direction: row; margin-bottom: 30px"><p style="margin-right: 48px">Институт:</p>'
    kafed = '<div style="display: flex; flex-direction: row; margin-bottom: 30px"><p style="margin-right: 50px">Кафедра:</p>'
    audit = '<div style="display: flex; flex-direction: row; margin-bottom: 20px"><p style="margin-right: 35px">Аудитория:</p>'
    select_place = '<select size="7" multiple="multiple">'
    for name in all_names:
        if name.id == 1:
            select_place += '<option>' + str(name.name) + '</option> '

    select_place += '</select>'
    inst += select_place + '</div>'
    html += inst
    select_place = '<select size="7" multiple="multiple">'
    for name in all_names:
        if name.id == 2 or name.id == 3:
            select_place += '<option>' + str(name.name) + '</option> '
    select_place += '</select>'
    kafed += select_place + '</div>'
    html += kafed
    select_place = '<select size="7" multiple="multiple">'
    for name in all_names:
        if name.id > 3:
            select_place += '<option>' + str(name.name) + '</option> '
    select_place += '</select>'
    audit += select_place + '</div>'
    html += audit + '<hr />'
    report = '''<h2>Форма отчёта</h2>
    <p>Пожалуйста, выберите желаемый формат получения отчёта:</p>
    <div style="display: flex; flex-direction: row; margin-bottom: 30px">
    <p style="margin-right: 50px">Формат:</p>
    <select size="6" multiple="multiple">
    <option>Онлайн</option>
    <option>.xlsx</option>
    <option>.html</option>
    </select>
    </div>
    <button style="margin-right: 20px">Отправить</button>
    <button>Сбросить</button>
    </div>
    '''
    html += report
    return HttpResponse(html)