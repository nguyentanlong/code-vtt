(function () {
    var POLL_INTERVAL_MS = 30000;
    var pollTimer = null;

    function callThongBao(method, data, onSuccess) {
        fetch('/ThongBao.aspx/' + method, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json; charset=utf-8' },
            body: JSON.stringify(data || {})
        })
            .then(function (res) { return res.json(); })
            .then(function (res) {
                var body = res.d || res;
                if (body && body.success) onSuccess(body.data);
            })
            .catch(function (err) { console.error('Lỗi ThongBao:', err); });
    }

    function updateBadge(count) {
        var $badge = document.getElementById('tbBadge');
        if (!$badge) return;
        if (count > 0) {
            $badge.style.display = 'inline-block';
            $badge.textContent = count > 99 ? '99+' : count;
        } else {
            $badge.style.display = 'none';
        }
    }

    function refreshUnreadCount() {
        callThongBao('GetUnreadCount', {}, updateBadge);
    }

    function escapeHtml(text) {
        if (!text) return '';
        return text.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    }

    function loadList() {
        var $list = document.getElementById('tbList');
        $list.innerHTML = '<div style="padding:20px; text-align:center; color:#999; font-size:13px;">Đang tải...</div>';

        callThongBao('GetList', {}, function (data) {
            if (!data || data.length === 0) {
                $list.innerHTML = '<div style="padding:20px; text-align:center; color:#999; font-size:13px;">Không có thông báo nào.</div>';
                return;
            }

            $list.innerHTML = '';
            data.forEach(function (item) {
                var div = document.createElement('div');
                div.style.cssText = 'padding:10px 14px; border-bottom:1px solid #f2f2f2; cursor:pointer;' +
                    (item.DaDoc ? 'background:#fff;' : 'background:#f0f7ff;');
                div.innerHTML =
                    '<div style="font-size:13px; font-weight:' + (item.DaDoc ? 'normal' : 'bold') + '; color:#222;">' + escapeHtml(item.TieuDe) + '</div>' +
                    '<div style="font-size:12px; color:#666; margin-top:2px;">' + escapeHtml(item.NoiDung) + '</div>' +
                    '<div style="font-size:11px; color:#aaa; margin-top:4px;">' + item.NgayTao + '</div>';

                div.addEventListener('click', function () {
                    callThongBao('MarkAsRead', { thongBaoId: item.ThongBaoID }, function () {
                        refreshUnreadCount();
                        if (item.DuongDan) window.location.href = item.DuongDan;
                    });
                });

                $list.appendChild(div);
            });
        });
    }

    function togglePanel() {
        var $panel = document.getElementById('tbPanel');
        var isOpen = $panel.style.display === 'block';
        $panel.style.display = isOpen ? 'none' : 'block';
        if (!isOpen) loadList();
    }

    document.addEventListener('DOMContentLoaded', function () {
        var $btn = document.getElementById('btnThongBao');
        var $markAll = document.getElementById('tbMarkAllRead');

        if ($btn) $btn.addEventListener('click', function (e) { e.stopPropagation(); togglePanel(); });

        document.addEventListener('click', function (e) {
            var $panel = document.getElementById('tbPanel');
            if ($panel && $panel.style.display === 'block' && !$panel.contains(e.target)) {
                $panel.style.display = 'none';
            }
        });

        if ($markAll) {
            $markAll.addEventListener('click', function (e) {
                e.preventDefault();
                callThongBao('MarkAllAsRead', {}, function () {
                    refreshUnreadCount();
                    loadList();
                });
            });
        }

        refreshUnreadCount();
        pollTimer = setInterval(function () {
            if (!document.hidden) refreshUnreadCount();
        }, POLL_INTERVAL_MS);

        document.addEventListener('visibilitychange', function () {
            if (!document.hidden) refreshUnreadCount();
        });
    });
})();