/*
dhtmlxScheduler v.4.3.0 Professional Evaluation

This software is covered by DHTMLX Evaluation License. Contact sales@dhtmlx.com to get Commercial or Enterprise license. Usage without proper license is prohibited.

(c) Dinamenta, UAB.
*/
Scheduler.plugin(function(e){e._props={},e.createUnitsView=function(t,a,n,i,r,s,d){"object"==typeof t&&(n=t.list,a=t.property,i=t.size||0,r=t.step||1,s=t.skip_incorrect,d=t.days||1,t=t.name),e._props[t]={map_to:a,options:n,step:r,position:0,days:d},i>e._props[t].options.length&&(e._props[t]._original_size=i,i=0),e._props[t].size=i,e._props[t].skip_incorrect=s||!1,e.date[t+"_start"]=e.date.day_start,e.templates[t+"_date"]=function(a,n){var i=e._props[t];return i.days>1?e.templates.week_date(a,n):e.templates.day_date(a)
},e._get_unit_index=function(t,a){var n=t.position||0,i=Math.floor((e._correct_shift(+a,1)-+e._min_date)/864e5),r=t.options.length;return i>=r&&(i%=r),n+i},e.templates[t+"_scale_text"]=function(e,t,a){return a.css?"<span class='"+a.css+"'>"+t+"</span>":t},e.templates[t+"_scale_date"]=function(a){var n=e._props[t],i=n.options;if(!i.length)return"";var r=e._get_unit_index(n,a),s=i[r];return e.templates[t+"_scale_text"](s.key,s.label,s)},e.templates[t+"_second_scale_date"]=function(t){return e.templates.week_scale_date(t)
},e.date["add_"+t]=function(a,n){return e.date.add(a,n*e._props[t].days,"day")},e.date["get_"+t+"_end"]=function(a){return e.date.add(a,(e._props[t].size||e._props[t].options.length)*e._props[t].days,"day")},e.attachEvent("onOptionsLoad",function(){for(var a=e._props[t],n=a.order={},i=a.options,r=0;r<i.length;r++)n[i[r].key]=r;a._original_size&&0===a.size&&(a.size=a._original_size,delete a.original_size),a.size>i.length?(a._original_size=a.size,a.size=0):a.size=a._original_size||a.size,e._date&&e._mode==t&&e.setCurrentView(e._date,e._mode)
}),e["mouse_"+t]=function(t){var a=e._props[this._mode];if(a){if(t=this._week_indexes_from_pos(t),this._drag_event||(this._drag_event={}),this._drag_id&&this._drag_mode&&(this._drag_event._dhx_changed=!0),this._drag_mode&&"new-size"==this._drag_mode){var n=e._get_event_sday(e._events[e._drag_id]);Math.floor(t.x/a.options.length)!=Math.floor(n/a.options.length)&&(t.x=n)}var i=t.x%a.options.length,r=Math.min(i+a.position,a.options.length-1);t.section=(a.options[r]||{}).key,t.x=Math.floor(t.x/a.options.length);
var s=this.getEvent(this._drag_id);this._update_unit_section({view:a,event:s,pos:t})}return t.force_redraw=!0,t},e.callEvent("onOptionsLoad",[])},e._update_unit_section=function(e){var t=e.view,a=e.event,n=e.pos;a&&(a[t.map_to]=n.section)},e.scrollUnit=function(t){var a=e._props[this._mode];a&&(a.position=Math.min(Math.max(0,a.position+t),a.options.length-a.size),this.setCurrentView())},function(){var t=function(t){var a=e._props[e._mode];if(a&&a.order&&a.skip_incorrect){for(var n=[],i=0;i<t.length;i++)"undefined"!=typeof a.order[t[i][a.map_to]]&&n.push(t[i]);
t.splice(0,t.length),t.push.apply(t,n)}return t},a=e._pre_render_events_table;e._pre_render_events_table=function(e,n){return e=t(e),a.apply(this,[e,n])};var n=e._pre_render_events_line;e._pre_render_events_line=function(e,a){return e=t(e),n.apply(this,[e,a])};var i=function(t,a){if(t&&"undefined"==typeof t.order[a[t.map_to]]){var n=e,i=864e5,r=Math.floor((a.end_date-n._min_date)/i);return t.options.length&&(a[t.map_to]=t.options[Math.min(r+t.position,t.options.length-1)].key),!0}},r=e._reset_scale,s=e.is_visible_events;
e.is_visible_events=function(t){var a=s.apply(this,arguments);if(a){var n=e._props[this._mode];if(n&&n.size){var i=n.order[t[n.map_to]];if(i<n.position||i>=n.size+n.position)return!1}}return a},e._reset_scale=function(){var t=e._props[this._mode],a=r.apply(this,arguments);if(t){this._max_date=this.date.add(this._min_date,t.days,"day");for(var n=this._els.dhx_cal_data[0].childNodes,i=0;i<n.length;i++)n[i].className=n[i].className.replace("_now","");var s=new Date;if(s.valueOf()>=this._min_date&&s.valueOf()<this._max_date){var d=864e5,_=Math.floor((s-e._min_date)/d),o=t.options.length,l=_*o,c=l+o;
for(i=l;c>i;i++)n[i].className=n[i].className.replace("dhx_scale_holder","dhx_scale_holder_now")}if(t.size&&t.size<t.options.length){var h=this._els.dhx_cal_header[0],u=document.createElement("DIV");t.position&&(u.className="dhx_cal_prev_button",u.style.cssText="left:1px;top:2px;position:absolute;",u.innerHTML="&nbsp;",h.firstChild.appendChild(u),u.onclick=function(){e.scrollUnit(-1*t.step)}),t.position+t.size<t.options.length&&(u=document.createElement("DIV"),u.className="dhx_cal_next_button",u.style.cssText="left:auto; right:0px;top:2px;position:absolute;",u.innerHTML="&nbsp;",h.lastChild.appendChild(u),u.onclick=function(){e.scrollUnit(t.step)
})}}return a};var d=e._reset_scale;e._reset_scale=function(){var t=e._props[this._mode],a=e.xy.scale_height;t&&t.days>1?this._header_resized||(this._header_resized=e.xy.scale_height,e.xy.scale_height=2*a):this._header_resized&&(e.xy.scale_height/=2,this._header_resized=!1),d.apply(this,arguments)};var _=e._render_x_header;e._render_x_header=function(t,a,n,i){var r=e._props[this._mode];if(!r||r.days<=1)return _.apply(this,arguments);if(r.days>1){var s=e.xy.scale_height;e.xy.scale_height=Math.ceil(s/2),_.call(this,t,a,n,i,Math.ceil(e.xy.scale_height));
var d=r.options.length;if((t+1)%d===0){var o=document.createElement("DIV");o.className="dhx_scale_bar dhx_second_scale_bar",this.templates[this._mode+"_second_scalex_class"]&&(o.className+=" "+this.templates[this._mode+"_scalex_class"](n));var l,c=this._cols[t]*d-1;l=d>1?this._colsS[t-(d-1)]-this.xy.scale_width-2:a;var h=this.date.add(this._min_date,Math.floor(t/d),"day");this.set_xy(o,c,this.xy.scale_height-2,l,0),o.innerHTML=this.templates[this._mode+"_second_scale_date"](h,this._mode),i.appendChild(o)
}e.xy.scale_height=s}};var o=e._get_event_sday;e._get_event_sday=function(t){var a=e._props[this._mode];if(a){if(a.days<=1)return i(a,t),this._get_section_sday(t[a.map_to]);var n=864e5,r=Math.floor((t.end_date.valueOf()-1-e._min_date.valueOf())/n),s=a.options.length,d=a.order[t[a.map_to]];return r*s+d-a.position}return o.call(this,t)},e._get_section_sday=function(t){var a=e._props[this._mode];return a.order[t]-a.position};var l=e.locate_holder_day;e.locate_holder_day=function(t,a,n){var r=e._props[this._mode];
if(r&&n){if(i(r,n),r.days<=1)return 1*r.order[n[r.map_to]]+(a?1:0)-r.position;var s=864e5,d=Math.floor((n.start_date.valueOf()-1-e._min_date.valueOf())/s),_=r.options.length,o=r.order[n[r.map_to]];return d*_+1*o+(a?1:0)-r.position}return l.apply(this,arguments)};var c=e._time_order;e._time_order=function(t){var a=e._props[this._mode];a?t.sort(function(e,t){return a.order[e[a.map_to]]>a.order[t[a.map_to]]?1:-1}):c.apply(this,arguments)};var h=e._pre_render_events_table;e._pre_render_events_table=function(t,a){function n(t){var a=e.date.add(t,1,"day");
return a=e.date.date_part(a)}var i=e._props[this._mode];if(i&&i.days>1&&!this.config.all_timed){for(var r={},s=0;s<t.length;s++){var d=t[s];if(this.isOneDayEvent(t[s])){var _=+e.date.date_part(new Date(d.start_date));r[_]||(r[_]=[]),r[_].push(d)}else{var o=new Date(Math.min(+d.end_date,+this._max_date)),l=new Date(Math.max(+d.start_date,+this._min_date));for(t.splice(s,1);+o>+l;){var c=e._lame_clone(d);c.start_date=l,c.end_date=n(c.start_date),l=e.date.add(l,1,"day");var _=+e.date.date_part(new Date(l));
r[_]||(r[_]=[]),r[_].push(c),t.splice(s,0,c),s++}s--}}var u=[];for(var s in r)u.splice.apply(u,[u.length-1,0].concat(h.apply(this,[r[s],a])));for(var s=0;s<u.length;s++)u[s]._first_chunk=u[s]._last_chunk=!1;u.sort(function(e,t){return e.start_date.valueOf()==t.start_date.valueOf()?e.id>t.id?1:-1:e.start_date>t.start_date?1:-1}),t=u}else t=h.apply(this,[t,a]);return t},e.attachEvent("onEventAdded",function(t,a){if(this._loading)return!0;for(var n in e._props){var i=e._props[n];"undefined"==typeof a[i.map_to]&&(a[i.map_to]=i.options[0].key)
}return!0}),e.attachEvent("onEventCreated",function(t,a){var n=e._props[this._mode];if(n&&a){var r=this.getEvent(t),s=this._mouse_coords(a);this._update_unit_section({view:n,event:r,pos:s}),i(n,r),this.event_updated(r)}return!0})}()});